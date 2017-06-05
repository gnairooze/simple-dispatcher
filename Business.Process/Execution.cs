using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDispatcher.Business.View.Request;

namespace SimpleDispatcher.Business.Process
{
    internal class Execution
    {
        #region attributes
        protected List<ExecutionWorker> _ExecutionWorkers = new List<ExecutionWorker>();
        #endregion

        #region constructors
        public Execution(ILogger.ILog logger)
        {
            this.Logger = logger;

            loadExecutionWorkers();
        }
        #endregion

        #region properties
        public ILogger.ILog Logger { private get; set; }
        #endregion

        public bool Execute(Business.View.Request.ListView request)
        {
            logInfo(string.Format("start Execute of request with ID {0}", request.ID));

            var worker = (from workerItem in this._ExecutionWorkers
                          where workerItem.BusinessID == request.Worker_BusinessID
                          select workerItem).Single();

            bool succeeded = worker.Execute(request);

            logInfo(string.Format("end Execute of request with ID {0} with status {1}", request.ID, succeeded?"succeeded":"failed"));

            return succeeded;
        }

        /// <summary>
        /// Load execution workers. We can have many types doing many businesses.
        /// </summary>
        protected void loadExecutionWorkers()
        {
            logInfo("start loadExecutionWorkers");

            this._ExecutionWorkers.Add(new TestExecutionWorker());

            logInfo("end loadExecutionWorkers");
        }

        private void logInfo(string what)
        {
            this.Logger.Log(ILogger.Priority.Info, this.GetType().ToString(), what, DateTime.Now);
        }
    }
}
