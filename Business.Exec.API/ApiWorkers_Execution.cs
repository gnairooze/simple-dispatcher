using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILogger;
using SimpleDispatcher.Business.Exec.Generic;

namespace SimpleDispatcher.Business.Exec.API
{
    public class ApiWorkers_Execution : Execution
    {
        #region constructors
        public ApiWorkers_Execution(ILog logger, Data.Model.QueueDbContext db) : base(logger, db)
        {
        }
        #endregion

        protected override void loadExecutionWorkers()
        {
            logInfo("start loadExecutionWorkers");

            var query = from dataModel in this.DB.Worker
                        select dataModel;

            _ExecutionWorkers = new List<IExecutionWorker>();

            foreach (var dataModel in query)
            {
                var worker = new API_ExecutionWorker()
                {
                    ViewModel = new View.Worker.ListView(dataModel)
                };
                worker.ExecutionCompleted += Worker_ExecutionCompleted;

                _ExecutionWorkers.Add(worker);
            }

            logInfo("workers read from DB to _ExecutionWorkers");

            logInfo("end loadExecutionWorkers");
        }

        protected override void loadExecutionWorkersAsync()
        {
            logInfo("start loadExecutionWorkersAsync");

            var query = from dataModel in this.DB.Worker
                        select dataModel;

            _ExecutionWorkersAsync = new List<IExecutionWorkerAsync>();

            foreach (var dataModel in query)
            {
                var worker = new API_ExecutionWorker()
                {
                    ViewModel = new View.Worker.ListView(dataModel)
                };
                worker.ExecutionCompleted += Worker_ExecutionCompleted;

                _ExecutionWorkersAsync.Add(worker);
            }

            logInfo("workers read from DB to _ExecutionWorkers");

            logInfo("end loadExecutionWorkersAsync");
        }

    }
}
