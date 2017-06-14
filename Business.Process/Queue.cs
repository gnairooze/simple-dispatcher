﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDispatcher.Business.View.Request;
using SimpleDispatcher.Business.Exec.Generic;
using System.Collections;

namespace SimpleDispatcher.Business.Process
{
    public class Queue
    {
        #region attributes
        protected List<Business.View.OperationSettings.ListView> _OperationsSettings;

        protected List<Data.Model.Request> _DataModelRequests;

        static Data.Model.QueueDbContext _db = new Data.Model.QueueDbContext();
        static Execution _exec = null;
        protected int _Counter;
        protected Guid _Group;
        #endregion

        #region constructors
        public Queue(int queueID, int topCount, ILogger.ILog logger, Vault.ExecType executionType )
        {
            this.Logger = logger;

            ExecFactory execfactory = new ExecFactory(this.Logger, Queue._db);
            Queue._exec = execfactory.GetExecution(executionType);
            Queue._exec.ExecutionCompleted += _exec_ExecutionCompleted;

            loadOperationsSettings();
            
            this.QueueID = queueID;
            this.TopCount = topCount;
        }

        private void _exec_ExecutionCompleted(object sender, ExecutionCompletedEventArgs e)
        {
            logInfo(string.Format("execution completed event triggered for request with ID {0} and status {1}", e.Request.ID, e.Succeeded ? "succeeded" : "failed"));

            logInfo(string.Format("start updating request with ID {0}", e.Request.ID));

            bool result = updateRequestStatus(e.Request, e.Succeeded);

            logInfo(string.Format("end updating request with ID {0}", e.Request.ID));

        }
        #endregion

        #region properties
        public int QueueID { get; set; }
        public int TopCount { get; set; }
        public ILogger.ILog Logger { private get; set; }
        #endregion

        #region entry methods
        public void Run(string clientIP)
        {
            this._Counter = 1;
            this._Group = Guid.NewGuid();

            loadRequests(clientIP);
            startProcessing(clientIP);
        }

        public void RunAsync()
        {
            loadRequests();
            startProcessingAsync();
        }
        #endregion
       
        #region internal methods
        private void startProcessing(string clientIP)
        {
            string who = getWho(System.Reflection.MethodInfo.GetCurrentMethod().Name, clientIP);

            int requestCounter = 0;
            int requestCount = this._DataModelRequests.Count();

            logInfo(who, string.Format("start startProcessing with {0} requests", requestCount), string.Empty, string.Empty, this._Counter++, this._Group);

            foreach (var dataModelRequest in this._DataModelRequests)
            {
                Business.View.Request.ListView request = new Business.View.Request.ListView(dataModelRequest);

                requestCounter++;

                logInfo(string.Format("start executing {0} of {1} requests with ID {2}", requestCounter, requestCount, request.ID), "request.ID", request.ID.ToString());

                bool succeeded = _exec.Execute(request);

                logInfo(string.Format("end executing {0} of {1} requests", requestCounter, requestCount), "request.ID", request.ID.ToString());
            }

            logInfo("end startProcessing", string.Empty, string.Empty);
        }

        private void startProcessingAsync()
        {
            int requestCounter = 0;
            int requestCount = this._DataModelRequests.Count();
            logInfo(string.Format("start startProcessing with {0} requests", requestCount));

            //Parallel.ForEach(this._DataModelRequests, dataModelRequest => SomeMethod(x));

            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (var dataModelRequest in this._DataModelRequests)
            {
                Business.View.Request.ListView request = new Business.View.Request.ListView(dataModelRequest);

                requestCounter++;
                logInfo(string.Format("start executing {0} of {1} requests with ID {2}", requestCounter, requestCount, request.ID));

                tasks.Add(_exec.ExecuteAsync(request));

                logInfo(string.Format("end executing {0} of {1} requests", requestCounter, requestCount));
            }

            StringBuilder errorLogBuilder = new StringBuilder();

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch(AggregateException ae)
            {
                logError(ae.GetBaseException(), ref errorLogBuilder);
                logError(ae, ref errorLogBuilder);

                ae.Handle((x) =>
                {
                    logError(x, ref errorLogBuilder);

                    return true;
                });

            }
            catch (Exception ex)
            {
                logError(ex, ref errorLogBuilder);
            }

            logInfo("end startProcessing");
        }

        private bool updateRequestStatus(ListView request, bool succeeded)
        {
            logInfo(string.Format("start updateRequestStatus of request with ID {0} to be {1}", request.ID, succeeded?"succeeded":"failed"));

            Business.View.OperationSettings.ListView operationSettings = this._OperationsSettings.Single(o => o.Operation == request.Operation);

            request.RemainingRetrials--;

            if (succeeded)
            {
                request.Status = View.Vault.RequestStatus.Succeeded;
            }
            else
            {
                if(request.RemainingRetrials <= 0)
                {
                    request.Status = View.Vault.RequestStatus.Failed;
                    request.NextRetryOn = null;
                    logInfo(string.Format("request with ID {0} consumed all retrial count. It is marked as failed", request.ID));
                }
                else
                {
                    request.Status = View.Vault.RequestStatus.Retrying;
                    request.NextRetryOn = DateTime.Now.AddSeconds(operationSettings.RetrialDelay);

                    logInfo(string.Format("request with ID {0} is marked for retrial. Remaining retirals is {1}. Next retry on {2}", request.ID, request.RemainingRetrials, request.NextRetryOn));
                }
            }

            Data.Model.Request dataModel = Queue._db.Request.Single(r => r.ID == request.ID);

            if(dataModel.ModifiedOn != request.ModifiedOn)
            {
                logInfo(string.Format("the request with ID {0} failed to be updated because it was modified during the processing. started processing with modified on {1} and now modified on is {2}", request.ID, request.ModifiedOn, dataModel.ModifiedOn));

                logInfo("end updateRequestStatus of request");

                return false;
            }

            request.CopyDataTo(dataModel);
            logInfo("data copied from the request list view to the data model");

            dataModel.ModifiedOn = DateTime.Now;

            Queue._db.SaveChanges();

            logInfo(string.Format("request with ID {0} updated in DB", request.ID));

            logInfo("end updateRequestStatus of request");

            return true;
            
        }

        private void loadRequests(string clientIP)
        {
            string who = getWho(System.Reflection.MethodInfo.GetCurrentMethod().Name, clientIP);

            logInfo(who, "start loadRequests", string.Empty, string.Empty, this._Counter++, this._Group);

            DateTime runDate = DateTime.Now;
            this._DataModelRequests = (from dataModel in Queue._db.Request
                        where dataModel.QueueID == this.QueueID
                        && 
                        (
                            dataModel.Status == (byte)Business.View.Vault.RequestStatus.NotProcessed 
                            || 
                            (
                                dataModel.Status == (byte)Business.View.Vault.RequestStatus.Retrying
                                &&
                                dataModel.NextRetryOn <= runDate
                            )
                         )
                        orderby dataModel.ID
                        select dataModel).Take(this.TopCount).ToList();

            logInfo(who, "load requests from DB to _DataModelRequests", string.Empty, string.Empty, this._Counter++, this._Group);

            logInfo(who, "end loadRequests", string.Empty, string.Empty, this._Counter++, this._Group);
        }

        private void loadOperationsSettings()
        {
            logInfo("start loadOperationsSettings");

            var query = from dataModel in Queue._db.OperationSettings
                        select dataModel;

            _OperationsSettings = new List<View.OperationSettings.ListView>();

            foreach (var dataModel in query)
            {
                _OperationsSettings.Add(new View.OperationSettings.ListView(dataModel));
            }

            logInfo("operations settings read from DB to _OperationsSettings");

            logInfo("end loadOperationsSettings");

        }

        private Guid logInfo(string who, string what, string refName, string refValue, int counter, Guid group)
        {
            ILogger.LogModel model = new ILogger.LogModel()
            {
                 Counter = counter,
                 Group = group,
                 LogType = ILogger.TypeOfLog.Info,
                 ReferenceName = refName,
                 ReferenceValue = refValue,
                 What = what,
                 When = DateTime.Now,
                 Who = who
            };

            return this.Logger.Log(model);
        }
        private Guid logError(string who, Exception ex, ref StringBuilder bld, string refName, string refValue, int counter, Guid group)
        {
            string errorDetails = extractError(ex, ref bld);

            ILogger.LogModel model = new ILogger.LogModel()
            {
                Counter = counter,
                Group = group,
                LogType = ILogger.TypeOfLog.Info,
                ReferenceName = refName,
                ReferenceValue = refValue,
                What = errorDetails,
                When = DateTime.Now,
                Who = who
            };

            return this.Logger.Log(model);
        }
        private string extractError(Exception ex, ref StringBuilder bld)
        {
            if (bld == null)
            {
                bld = new StringBuilder();
            }

            if (ex.Data != null && ex.Data.Count > 0)
            {
                foreach (DictionaryEntry item in ex.Data)
                {
                    bld.AppendLine(String.Format("Data Key:{0}. Data Value:{1}.", item.Key, item.Value));
                }
            }

            bld.AppendLine(String.Format("Message:{0}.", ex.Message));
            bld.AppendLine(String.Format("StackTrace:{0}.", ex.StackTrace));

            if (ex.InnerException != null)
            {
                extractError(ex.InnerException, ref bld);
            }

            return bld.ToString();
        }
        private string getWho(string method, string clientIp)
        {
            string name = System.Net.Dns.GetHostName();
            System.Net.IPAddress[] ips = System.Net.Dns.GetHostAddresses(name);

            List<string> allIPs = new List<string>();

            if (ips != null)
            {
                foreach (System.Net.IPAddress ip in ips)
                {
                    allIPs.Add(ip.ToString());
                }
            }

            string who = string.Format("Client IP:{0} | HostName:{1} | IPs:{2} | Location:{3} | Assembly:{4} | Class:{5} | Method:{6}", clientIp, name, string.Join(",", allIPs.ToArray()), System.Reflection.Assembly.GetExecutingAssembly().Location, System.Reflection.Assembly.GetExecutingAssembly().FullName, this.GetType().ToString(), method);

            return who;
        }
        #endregion
    }
}
