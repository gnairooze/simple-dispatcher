using ILogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.Process
{
    internal class ExecFactory
    {
        #region constructors
        public ExecFactory(ILog logger, Data.Model.QueueDbContext db)
        {
            this.Logger = logger;
            this.DB = db;
        }
        #endregion

        #region properties
        public ILogger.ILog Logger { private get; set; }
        public Data.Model.QueueDbContext DB { private get; set; }
        #endregion

        public Exec.Generic.Execution GetExecution(string module, Vault.ExecType executionType)
        {
            Exec.Generic.Execution exec = null;

            switch (executionType)
            {
                case Vault.ExecType.Generic:
                    exec = new Exec.Generic.Execution(module, this.Logger, this.DB);
                    break;
                case Vault.ExecType.ApiWorker:
                    exec = new Exec.API.ApiWorkers_Execution(module, this.Logger, this.DB);
                    break;
                default:
                    exec = new Exec.Generic.Execution(module, this.Logger, this.DB);
                    break;
            }

            return exec;
        }
    }
}
