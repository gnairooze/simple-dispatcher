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
                _ExecutionWorkers.Add(new API_ExecutionWorker()
                {
                    ViewModel = new View.Worker.ListView(dataModel)
                });
            }

            logInfo("workers read from DB to _ExecutionWorkers");

            logInfo("end loadExecutionWorkers");
        }

    }
}
