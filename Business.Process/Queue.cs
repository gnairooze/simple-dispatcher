using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDispatcher.Business.View.Request;

namespace SimpleDispatcher.Business.Process
{
    public class Queue
    {
        #region attributes
        protected List<Business.View.OperationSettings.ListView> _OperationsSettings;
        protected List<Business.View.Worker.ListView> _Workers;
        protected List<Data.Model.Request> _DataModelRequests;

        static Data.Model.QueueDbContext db = new Data.Model.QueueDbContext();
        static Execution exec = null;
        #endregion

        #region constructors
        public Queue(int queueID, int topCount, ILogger.ILog logger)
        {
            this.Logger = logger;
            Queue.exec = new Execution(this.Logger);

            loadWorkers();
            loadOperationsSettings();
            
            this.QueueID = queueID;
            this.TopCount = topCount;
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

                bool succeeded = exec.Execute(request);

                logInfo(string.Format("end executing {0} of {1} requests", requestCounter, requestCount));

                logInfo(string.Format("start updating {0} of {1} requests with ID {2}", requestCounter, requestCount, request.ID));

                updateRequestStatus(request, succeeded);

                logInfo(string.Format("end updating {0} of {1} requests", requestCounter, requestCount));
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
                request.Status = View.BusinessVault.RequestStatus.Succeeded;
            }
            else
            {
                if(request.RemainingRetrials <= 0)
                {
                    request.Status = View.BusinessVault.RequestStatus.Failed;
                    request.NextRetryOn = null;
                    logInfo(string.Format("request with ID {0} consumed all retrial count. It is marked as failed", request.ID));
                }
                else
                {
                    request.Status = View.BusinessVault.RequestStatus.Retrying;
                    request.NextRetryOn = DateTime.Now.AddSeconds(operationSettings.RetrialDelay);

                    logInfo(string.Format("request with ID {0} is marked for retrial. Remaining retirals is {1}. Next retry on {2}", request.ID, request.RemainingRetrials, request.NextRetryOn));
                }
            }

            Data.Model.Request dataModel = Queue.db.Request.Single(r => r.ID == request.ID);

            if(dataModel.ModifiedOn != request.ModifiedOn)
            {
                logInfo(string.Format("the request with ID {0} failed to be updated because it was modified during the processing. started processing with modified on {1} and now modified on is {2}", request.ID, request.ModifiedOn, dataModel.ModifiedOn));

                logInfo("end updateRequestStatus of request");

                return false;
            }

            request.CopyDataTo(dataModel);
            logInfo("data copied from the request list view to the data model");

            dataModel.ModifiedOn = DateTime.Now;

            Queue.db.SaveChanges();

            logInfo(string.Format("request with ID {0} updated in DB", request.ID));

            logInfo("end updateRequestStatus of request");

            return true;
            
        }

        private void loadRequests()
        {
            logInfo("start loadRequests");

            DateTime runDate = DateTime.Now;
            this._DataModelRequests = (from dataModel in Queue.db.Request
                        where dataModel.QueueID == this.QueueID
                        && 
                        (
                            dataModel.Status == (byte)Business.View.BusinessVault.RequestStatus.NotProcessed 
                            || 
                            (
                                dataModel.Status == (byte)Business.View.BusinessVault.RequestStatus.Retrying
                                &&
                                dataModel.NextRetryOn <= runDate
                            )
                         )
                        orderby dataModel.ID
                        select dataModel).Take(this.TopCount).ToList();

            logInfo("load requests from DB to _DataModelRequests");

            logInfo("end loadRequests");
        }

        private void loadWorkers()
        {
            logInfo("start loadWorkers");

            var query = from dataModel in Queue.db.Worker
                        select dataModel;

            _Workers = new List<View.Worker.ListView>();

            foreach (var dataModel in query)
            {
                _Workers.Add(new View.Worker.ListView(dataModel));
            }

            logInfo("workers read from DB to _Workers");

            logInfo("end loadWorkers");
        }
        private void loadOperationsSettings()
        {
            logInfo("start loadOperationsSettings");

            var query = from dataModel in Queue.db.OperationSettings
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
    }
}
