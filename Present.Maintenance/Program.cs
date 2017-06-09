using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Present.Maintenance
{
    class Program
    {
        static ConsoleLogger.SimpleLogger _Logger = new ConsoleLogger.SimpleLogger();

        static void Main(string[] args)
        {
            DateTime runDate = DateTime.Now;


            logInfo(string.Format("EnableDeleteCancelled: {0}", Present.Maintenance.Properties.Settings.Default.EnableDeleteCancelled));
            if (Present.Maintenance.Properties.Settings.Default.EnableDeleteCancelled)
            {
                logInfo(string.Format("start DeleteCancelledRequests"));
                Business.Maintenance.Manager.DeleteCancelledRequests(runDate, Present.Maintenance.Properties.Settings.Default.DeleteChunk, Present.Maintenance.Properties.Settings.Default.DeleteTimeout);
                logInfo(string.Format("end DeleteCancelledRequests"));
            }

            logInfo(string.Format("EnableDeleteFailed: {0}", Present.Maintenance.Properties.Settings.Default.EnableDeleteFailed));
            if (Present.Maintenance.Properties.Settings.Default.EnableDeleteFailed)
            {
                logInfo(string.Format("start DeleteFailedRequests"));
                Business.Maintenance.Manager.DeleteFailedRequests(runDate, Present.Maintenance.Properties.Settings.Default.DeleteChunk, Present.Maintenance.Properties.Settings.Default.DeleteTimeout);
                logInfo(string.Format("end DeleteFailedRequests"));
            }

            logInfo(string.Format("EnableDeleteNotProcessed: {0}", Present.Maintenance.Properties.Settings.Default.EnableDeleteNotProcessed));
            if (Present.Maintenance.Properties.Settings.Default.EnableDeleteNotProcessed)
            {
                logInfo(string.Format("start DeleteNotProcessedRequests"));
                Business.Maintenance.Manager.DeleteNotProcessedRequests(runDate, Present.Maintenance.Properties.Settings.Default.DeleteChunk, Present.Maintenance.Properties.Settings.Default.DeleteTimeout);
                logInfo(string.Format("end DeleteNotProcessedRequests"));
            }

            logInfo(string.Format("EnableDeleteRetrying: {0}", Present.Maintenance.Properties.Settings.Default.EnableDeleteRetrying));
            if (Present.Maintenance.Properties.Settings.Default.EnableDeleteRetrying)
            {
                logInfo(string.Format("start DeleteRetryingRequests"));
                Business.Maintenance.Manager.DeleteRetryingRequests(runDate, Present.Maintenance.Properties.Settings.Default.DeleteChunk, Present.Maintenance.Properties.Settings.Default.DeleteTimeout);
                logInfo(string.Format("end DeleteRetryingRequests"));
            }

            logInfo(string.Format("EnableDeleteSkipped: {0}", Present.Maintenance.Properties.Settings.Default.EnableDeleteSkipped));
            if (Present.Maintenance.Properties.Settings.Default.EnableDeleteSkipped)
            {
                logInfo(string.Format("start DeleteSkippedRequests"));
                Business.Maintenance.Manager.DeleteSkippedRequests(runDate, Present.Maintenance.Properties.Settings.Default.DeleteChunk, Present.Maintenance.Properties.Settings.Default.DeleteTimeout);
                logInfo(string.Format("end DeleteSkippedRequests"));
            }

            logInfo(string.Format("EnableDeleteSucceeded: {0}", Present.Maintenance.Properties.Settings.Default.EnableDeleteSucceeded));
            if (Present.Maintenance.Properties.Settings.Default.EnableDeleteSucceeded)
            {
                logInfo(string.Format("start DeleteSucceededRequests"));
                Business.Maintenance.Manager.DeleteSucceededRequests(runDate, Present.Maintenance.Properties.Settings.Default.DeleteChunk, Present.Maintenance.Properties.Settings.Default.DeleteTimeout);
                logInfo(string.Format("end DeleteSucceededRequests"));
            }
        }

        private static void logInfo(string what)
        {
            _Logger.Log(ILogger.Priority.Info, "SimpleDispatcher.Present.Maintenance.Program", what, DateTime.Now);
        }
    }
}
