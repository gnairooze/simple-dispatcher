﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDispatcher.Business.View.Request;
using ILogger;

namespace SimpleDispatcher.Business.Exec.Generic
{
    public class Execution
    {
        public event EventHandler<ExecutionCompletedEventArgs> ExecutionCompleted;

        #region attributes
        protected List<IExecutionWorker> _ExecutionWorkers = new List<IExecutionWorker>();
        protected List<IExecutionWorkerAsync> _ExecutionWorkersAsync = new List<IExecutionWorkerAsync>();
        private Guid _ExecutionBusinessID = Guid.NewGuid();
        private int _LoggerCounter = 0;
        private string _Module;
        #endregion

        #region constructors
        public Execution(string module, ILog logger, Data.Model.QueueDbContext db)
        {
            this.Logger = logger;
            this.DB = db;
            this._Module = module;

            loadExecutionWorkers();
            loadExecutionWorkersAsync();
        }
        #endregion

        #region properties
        public ILogger.ILog Logger { protected get; set; }
        public Data.Model.QueueDbContext DB { protected get; set; }
        #endregion

        public bool Execute(Business.View.Request.ListView request)
        {
            logInfo(string.Format("start Execute of request with ID {0}", request.ID));

            var worker = (from workerItem in this._ExecutionWorkers
                          where workerItem.ViewModel.BusinessID == request.Worker_BusinessID
                          select workerItem).Single();

            bool succeeded = worker.Execute(request);

            logInfo(string.Format("end Execute of request with ID {0} with status {1}", request.ID, succeeded?"succeeded":"failed"));

            return succeeded;
        }
        //need some tuning as generates a lot of entity framework errors
        public async Task<bool> ExecuteAsync(Business.View.Request.ListView request)
        {
            logInfo(string.Format("start ExecuteAsync of request with ID {0}", request.ID));

            var worker = (from workerItem in this._ExecutionWorkersAsync
                          where workerItem.ViewModel.BusinessID == request.Worker_BusinessID
                          select workerItem).Single();

            worker.ExecutionCompleted += Worker_ExecutionCompleted;

            bool succeeded = await worker.ExecuteAsync(request);

            logInfo(string.Format("end ExecuteAsync of request with ID {0} with status {1}", request.ID, succeeded ? "succeeded" : "failed"));

            return succeeded;
        }

        protected void Worker_ExecutionCompleted(object sender, ExecutionCompletedEventArgs e)
        {
            //bubble the event up to queue
            OnExecutionCompletion(new ExecutionCompletedEventArgs()
            {
                Request = e.Request,
                Worker = e.Worker,
                Succeeded = e.Succeeded
            });
        }

        public void OnExecutionCompletion(ExecutionCompletedEventArgs e)
        {
            EventHandler<ExecutionCompletedEventArgs> handler = ExecutionCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        /// <summary>
        /// Load execution workers. We can have many types doing many businesses.
        /// </summary>
        protected virtual void loadExecutionWorkers()
        {
            logInfo("start loadExecutionWorkers");

            var worker = new TestExecutionWorker();
            worker.ExecutionCompleted += Worker_ExecutionCompleted;

            this._ExecutionWorkers.Add(worker);

            logInfo("end loadExecutionWorkers");
        }

        protected virtual void loadExecutionWorkersAsync()
        {
            logInfo("start loadExecutionWorkers");

            var worker = new TestExecutionWorkerAsync();
            worker.ExecutionCompleted += Worker_ExecutionCompleted;

            this._ExecutionWorkersAsync.Add(worker);

            logInfo("end loadExecutionWorkers");
        }

        protected void logInfo(string what)
        {
            ILogger.LogModel logModel = new LogModel()
            {
                BusinessID = this._ExecutionBusinessID,
                Counter = this._LoggerCounter++,
                CreatedOn = DateTime.Now,
                Group = this._ExecutionBusinessID,
                LogType = TypeOfLog.Info,
                Module = this._Module,
                ReferenceName = "Class",
                ReferenceValue = "Business.Exec.Generic",
                What = what,
                When = DateTime.Now,
                Who = "System"
            };
            this.Logger.Log(logModel);
        }
    }
}
