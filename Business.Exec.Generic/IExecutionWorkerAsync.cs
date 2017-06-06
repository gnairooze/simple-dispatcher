using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.Exec.Generic
{
    public interface IExecutionWorkerAsync:IExecutionWorker
    {
        Task<bool> ExecuteAsync(Business.View.Request.ListView request);
    }
}
