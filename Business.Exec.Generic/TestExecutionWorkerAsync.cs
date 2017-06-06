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
            if (request.ID % 13 != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ExecuteAsync(View.Request.ListView request)
        {
            return await Task.FromResult<bool>(Execute(request));
        }
    }
}
