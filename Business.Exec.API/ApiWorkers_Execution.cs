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
            string who = getWho(System.Reflection.MethodInfo.GetCurrentMethod().Name, string.Empty);

            logInfo(who, "start loadExecutionWorkers", string.Empty, string.Empty, this._Counter++, this._Group);

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

            logInfo(who, "workers read from DB to _ExecutionWorkers", string.Empty, string.Empty, this._Counter++, this._Group);

            logInfo(who, "end loadExecutionWorkers", string.Empty, string.Empty, this._Counter++, this._Group);
        }

        protected override void loadExecutionWorkersAsync()
        {
            string who = getWho(System.Reflection.MethodInfo.GetCurrentMethod().Name, string.Empty);

            logInfo(who, "start loadExecutionWorkersAsync", string.Empty, string.Empty, this._Counter++, this._Group);

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

            logInfo(who, "workers read from DB to _ExecutionWorkers", string.Empty, string.Empty, this._Counter++, this._Group);

            logInfo(who, "end loadExecutionWorkersAsync", string.Empty, string.Empty, this._Counter++, this._Group);
        }

    }
}
