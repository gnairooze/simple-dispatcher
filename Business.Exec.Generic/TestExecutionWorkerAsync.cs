using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDispatcher.Business.View.Request;
using SimpleDispatcher.Business.View.Worker;

namespace SimpleDispatcher.Business.Exec.Generic
{
    internal class TestExecutionWorkerAsync : IExecutionWorkerAsync
    {
        public event EventHandler<ExecutionCompletedEventArgs> ExecutionCompleted;

        #region properties
        public View.Worker.ListView ViewModel { get; set; }
        #endregion

        #region consturctors
        public TestExecutionWorkerAsync()
        {
            this.ViewModel = new View.Worker.ListView()
            {
                BusinessID = Guid.Parse("82A406F4-1332-4DBA-8CA4-D945B5B2AED8"),
                ID = 2
            };
        }
        #endregion

        public bool Execute(Business.View.Request.ListView request)
        {
            bool result = false;
            if (request.ID % 13 != 0)
            {
                result = true;
            }

            //trigger the event
            OnExecutionCompletion(new ExecutionCompletedEventArgs() {
                Request = request,
                Worker = this,
                Succeeded = result
            });
            return result;
        }

        public async Task<bool> ExecuteAsync(View.Request.ListView request)
        {
            return await Task.FromResult<bool>(Execute(request));
        }

        public void OnExecutionCompletion(ExecutionCompletedEventArgs e)
        {
            EventHandler<ExecutionCompletedEventArgs> handler = ExecutionCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
