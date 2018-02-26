using System;
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

        protected Data.Model.QueueDbContext _db = new Data.Model.QueueDbContext();
        protected Execution _exec = null;
        protected int _Counter;
        protected Guid _Group;
        
        #endregion

        #region constructors
        public Queue(string clientIP, int queueID, int topCount, ILogger.ILog logger, Vault.ExecType executionType )
        {
            this.Logger = logger;

            ExecFactory execfactory = new ExecFactory(this.Logger, this._db);
            this._exec = execfactory.GetExecution(executionType);
            this._exec.ExecutionCompleted += _exec_ExecutionCompleted;

            loadOperationsSettings(clientIP);
            
            this.QueueID = queueID;
            this.TopCount = topCount;
        }

        private void _exec_ExecutionCompleted(object sender, ExecutionCompletedEventArgs e)
        {
            logInfo(e.ClientIP, string.Format("execution completed event triggered for request with ID {0} and status {1}", e.Request.ID, e.Succeeded ? "succeeded" : "failed"));

            logInfo(e.ClientIP, string.Format("start updating request with ID {0}", e.Request.ID));

            bool result = updateRequestStatus(e.ClientIP, e.Request, e.Succeeded);

            logInfo(e.ClientIP, string.Format("end updating request with ID {0}", e.Request.ID));

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
            if (this._db == null)
            {
                this._db = new Data.Model.QueueDbContext();
            }

            this._Counter = 1;
            this._Group = Guid.NewGuid();

            loadRequests(clientIP);
            startProcessing(clientIP);

            this._db.Dispose();
            this._db = null;
        }

        //need some tuning as generates a lot of entity framework errors
        public void RunAsync(string clientIP)
        {
            if (this._db == null)
            {
                this._db = new Data.Model.QueueDbContext();
            }

            loadRequests(clientIP);
            startProcessingAsync(clientIP);

            this._db.Dispose();
            this._db = null;
        }
        #endregion
       
        #region internal methods
        private void startProcessing(string clientIP)
        {
            int requestCounter = 0;
            int requestCount = this._DataModelRequests.Count();

            logInfo(clientIP, string.Format("start startProcessing with {0} requests", requestCount));

            foreach (var dataModelRequest in this._DataModelRequests)
            {
                Business.View.Request.ListView request = new Business.View.Request.ListView(dataModelRequest);

                requestCounter++;

                logInfo(clientIP, string.Format("start executing {0} of {1} requests with ID {2}", requestCounter, requestCount, request.ID));

                bool succeeded = _exec.Execute(request);

                logInfo(clientIP, string.Format("end executing {0} of {1} requests", requestCounter, requestCount));
            }

            logInfo(clientIP, "end startProcessing");
        }
        //need some tuning as generates a lot of entity framework errors
        private void startProcessingAsync(string clientIP)
        {
            int requestCounter = 0;
            int requestCount = this._DataModelRequests.Count();
            logInfo(clientIP, string.Format("start startProcessing with {0} requests", requestCount));

            //Parallel.ForEach(this._DataModelRequests, dataModelRequest => SomeMethod(x));

            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (var dataModelRequest in this._DataModelRequests)
            {
                Business.View.Request.ListView request = new Business.View.Request.ListView(dataModelRequest);

                requestCounter++;
                logInfo(clientIP, string.Format("start executing {0} of {1} requests with ID {2}", requestCounter, requestCount, request.ID));

                tasks.Add(_exec.ExecuteAsync(request));

                logInfo(clientIP, string.Format("end executing {0} of {1} requests", requestCounter, requestCount));
            }

            StringBuilder errorLogBuilder = new StringBuilder();

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch(AggregateException ae)
            {
                logError(clientIP, ae.GetBaseException(), ref errorLogBuilder);
                logError(clientIP, ae, ref errorLogBuilder);

                ae.Handle((x) =>
                {
                    logError(clientIP, x, ref errorLogBuilder);

                    return true;
                });

            }
            catch (Exception ex)
            {
                logError(clientIP, ex, ref errorLogBuilder);
            }

            logInfo(clientIP, "end startProcessing");
        }

        private bool updateRequestStatus(string clientIP, ListView request, bool succeeded)
        {
            logInfo(clientIP, string.Format("start updateRequestStatus of request with ID {0} to be {1}", request.ID, succeeded?"succeeded":"failed"));

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
                    logInfo(clientIP, string.Format("request with ID {0} consumed all retrial count. It is marked as failed", request.ID));
                }
                else
                {
                    request.Status = View.Vault.RequestStatus.Retrying;
                    request.NextRetryOn = DateTime.Now.AddSeconds(operationSettings.RetrialDelay);

                    logInfo(clientIP, string.Format("request with ID {0} is marked for retrial. Remaining retirals is {1}. Next retry on {2}", request.ID, request.RemainingRetrials, request.NextRetryOn));
                }
            }

            Data.Model.Request dataModel = this._db.Request.Single(r => r.ID == request.ID);

            if(dataModel.ModifiedOn != request.ModifiedOn)
            {
                logInfo(clientIP, string.Format("the request with ID {0} failed to be updated because it was modified during the processing. started processing with modified on {1} and now modified on is {2}", request.ID, request.ModifiedOn, dataModel.ModifiedOn));

                logInfo(clientIP, "end updateRequestStatus of request");

                return false;
            }

            request.CopyDataTo(dataModel);
            logInfo(clientIP, "data copied from the request list view to the data model");

            dataModel.ModifiedOn = DateTime.Now;

            this._db.SaveChanges();

            logInfo(clientIP, string.Format("request with ID {0} updated in DB", request.ID));

            logInfo(clientIP, "end updateRequestStatus of request");

            return true;
            
        }

        private void loadRequests(string clientIP)
        {
            logInfo(clientIP, "start loadRequests");

            DateTime runDate = DateTime.Now;
            this._DataModelRequests = (from dataModel in this._db.Request
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

            logInfo(clientIP, "load requests from DB to _DataModelRequests");

            logInfo(clientIP, "end loadRequests");
        }

        private void loadOperationsSettings(string clientIP)
        {
            logInfo(clientIP, "start loadOperationsSettings");

            var query = from dataModel in this._db.OperationSettings
                        select dataModel;

            _OperationsSettings = new List<View.OperationSettings.ListView>();

            foreach (var dataModel in query)
            {
                _OperationsSettings.Add(new View.OperationSettings.ListView(dataModel));
            }

            logInfo(clientIP, "operations settings read from DB to _OperationsSettings");

            logInfo(clientIP, "end loadOperationsSettings");

        }

        private Guid logInfo(string clientIP, string what)
        {
            string who = getWho(System.Reflection.MethodInfo.GetCurrentMethod().Name, clientIP);

            ILogger.LogModel model = new ILogger.LogModel()
            {
                 Counter = this._Counter,
                 Group = this._Group,
                 LogType = ILogger.TypeOfLog.Info,
                 ReferenceName = "Class",
                 ReferenceValue = "Business.Process",
                 What = what,
                 When = DateTime.Now,
                 Who = who
            };

            return this.Logger.Log(model);
        }
        private Guid logError(string clientIP, Exception ex, ref StringBuilder bld)
        {
            string who = getWho(System.Reflection.MethodInfo.GetCurrentMethod().Name, clientIP);

            string errorDetails = extractError(ex, ref bld);

            ILogger.LogModel model = new ILogger.LogModel()
            {
                Counter = this._Counter,
                Group = this._Group,
                LogType = ILogger.TypeOfLog.Error,
                ReferenceName = "Class",
                ReferenceValue = "Business.Process",
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
