using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.Exec.Generic
{
    public interface IExecutionWorker
    {

        #region properties
        View.Worker.ListView ViewModel { get; set; }
        #endregion

        bool Execute(Business.View.Request.ListView request);

        event EventHandler<View.Request.ListView> ExecutionCompleted;
    }
}
