using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.Maintenance
{
    public class Manager
    {
        private static Data.Model.QueueDbContext _DB = new Data.Model.QueueDbContext();

        public static void DeleteSucceededRequests(DateTime runDate, int chunk, int timeout)
        {
            _DB.Database.CommandTimeout = timeout;
            _DB.DeleteRequestsSucceeded(runDate, chunk);
            _DB.Database.CommandTimeout = null;
        }
        public static void DeleteNotProcessedRequests(DateTime runDate, int chunk, int timeout)
        {
            _DB.Database.CommandTimeout = timeout;
            _DB.DeleteRequestsNotProcessed(runDate, chunk);
            _DB.Database.CommandTimeout = null;
        }
        public static void DeleteFailedRequests(DateTime runDate, int chunk, int timeout)
        {
            _DB.Database.CommandTimeout = timeout;
            _DB.DeleteRequestsFailed(runDate, chunk);
            _DB.Database.CommandTimeout = null;
        }
        public static void DeleteRetryingRequests(DateTime runDate, int chunk, int timeout)
        {
            _DB.Database.CommandTimeout = timeout;
            _DB.DeleteRequestsRetrying(runDate, chunk);
            _DB.Database.CommandTimeout = null;
        }
        public static void DeleteSkippedRequests(DateTime runDate, int chunk, int timeout)
        {
            _DB.Database.CommandTimeout = timeout;
            _DB.DeleteRequestsSkipped(runDate, chunk);
            _DB.Database.CommandTimeout = null;
        }
        public static void DeleteCancelledRequests(DateTime runDate, int chunk, int timeout)
        {
            _DB.Database.CommandTimeout = timeout;
            _DB.DeleteRequestsCancelled(runDate, chunk);
            _DB.Database.CommandTimeout = null;
        }
    }
}
