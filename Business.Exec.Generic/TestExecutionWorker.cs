using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDispatcher.Business.View.Request;

namespace SimpleDispatcher.Business.Exec.Generic
{
    internal class TestExecutionWorker:IExecutionWorker
    {

        #region properties
        public View.Worker.ListView ViewModel { get; set; }
        #endregion

        #region consturctors
        public TestExecutionWorker()
        {
            this.ViewModel = new View.Worker.ListView()
            {
                BusinessID = Guid.Parse("A92DF39B-EEC6-4967-989C-9C3177BE1231"),
                ID = 1
            };
        }

        public event EventHandler<ExecutionCompletedEventArgs> ExecutionCompleted;
        
        #endregion

        public bool Execute(Business.View.Request.ListView request)
        {
            bool result = false;

            if(request.ID % 13 != 0)
            {
                result = true;
            }

            //trigger the event
            OnExecutionCompletion(new ExecutionCompletedEventArgs()
            {
                Request = request,
                Worker = this,
                Succeeded = result
            });

            return result;
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
