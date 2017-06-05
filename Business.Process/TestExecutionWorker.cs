using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDispatcher.Business.View.Request;

namespace SimpleDispatcher.Business.Process
{
    internal class TestExecutionWorker:ExecutionWorker
    {
        public TestExecutionWorker()
        {
            this.ID = 1;
            this.BusinessID = Guid.Parse("A92DF39B-EEC6-4967-989C-9C3177BE1231");
        }
        public override bool Execute(ListView request)
        {
            if(request.ID % 13 != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
