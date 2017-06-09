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

        static Data.Model.QueueDbContext _db = new Data.Model.QueueDbContext();
        static Execution _exec = null;
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

        public void Run()
        {
            loadRequests();
            startProcessing();
        }

        public void RunAsync()
        {
            loadRequests();
            startProcessingAsync();
        }

        private void startProcessing()
        {
            int requestCounter = 0;
            int requestCount = this._DataModelRequests.Count();
            logInfo(string.Format("start startProcessing with {0} requests", requestCount));

            foreach (var dataModelRequest in this._DataModelRequests)
            {
                Business.View.Request.ListView request = new Business.View.Request.ListView(dataModelRequest);

                requestCounter++;
                logInfo(string.Format("start executing {0} of {1} requests with ID {2}", requestCounter, requestCount, request.ID));

                bool succeeded = _exec.Execute(request);

                logInfo(string.Format("end executing {0} of {1} requests", requestCounter, requestCount));

                logInfo(string.Format("start updating {0} of {1} requests with ID {2}", requestCounter, requestCount, request.ID));

                logInfo(string.Format("end updating {0} of {1} requests", requestCounter, requestCount));
            }

            logInfo("end startProcessing");
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

        private string extractError(Exception ex, ref StringBuilder bld)
        {
            if(bld == null)
            {
                bld = new StringBuilder();
            }

            if(ex.Data != null && ex.Data.Count > 0)
            {
                foreach (DictionaryEntry item in ex.Data)
                {
                    bld.AppendLine(String.Format("Data Key:{0}. Data Value:{1}.",item.Key, item.Value));
                }
            }

            bld.AppendLine(String.Format("Message:{0}.", ex.Message));
            bld.AppendLine(String.Format("StackTrace:{0}.", ex.StackTrace));

            if(ex.InnerException != null)
            {
                extractError(ex.InnerException, ref bld);
            }

            return bld.ToString();
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

        private void loadRequests()
        {
            logInfo("start loadRequests");

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

            logInfo("load requests from DB to _DataModelRequests");

            logInfo("end loadRequests");
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

        private void logInfo(string what)
        {
            this.Logger.Log(ILogger.Priority.Info, this.GetType().ToString(), what, DateTime.Now);
        }

        private void logError(Exception ex, ref StringBuilder bld)
        {
            string errorDetails = extractError(ex, ref bld);
            this.Logger.Log(ILogger.Priority.Error, this.GetType().ToString(), errorDetails, DateTime.Now);
        }
    }
}
