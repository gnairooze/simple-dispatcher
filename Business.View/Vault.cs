using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.View
{
    public class Vault
    {
        public enum RequestStatus
        {
            NotSet = 0,
            NotProcessed = 1,
            Succeeded = 2,
            Failed = 3,
            Retrying = 4,
            Skipped = 5,
            Cancelled = 6
        }
    }
}
