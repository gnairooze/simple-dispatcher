using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Present.Maintenance
{
    class Program
    {
        static ConsoleLogger.SimpleLogger _Logger = new ConsoleLogger.SimpleLogger() {
            CanAddError = Present.Maintenance.Properties.Settings.Default.CanAddError,
            CanAddInfo = Present.Maintenance.Properties.Settings.Default.CanAddInfo,
            CanAddWarning = Present.Maintenance.Properties.Settings.Default.CanAddWarning
        };

        static int _Counter = 0;
        static Guid _BusinessID = Guid.NewGuid();

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
            string who = getWho(System.Reflection.MethodInfo.GetCurrentMethod().Name, string.Empty);

            ILogger.LogModel model = new ILogger.LogModel()
            {
                 Counter = _Counter,
                 Group = _BusinessID,
                 LogType = ILogger.TypeOfLog.Info,
                 ReferenceName = "Class",
                 ReferenceValue = "Present.Maintenance.Program",
                 What = what,
                 When = DateTime.Now,
                 Who = who
            };

            _Logger.Log(model);
        }

        private static string getWho(string method, string clientIp)
        {
            string name = System.Net.Dns.GetHostName();
            System.Net.IPAddress[] ips = System.Net.Dns.GetHostAddresses(name);

            List<string> allIPs = new List<string>();

            if (ips != null)
            {
                foreach (System.Net.IPAddress ip in ips)
                {
                    allIPs.Add(ip.ToString());
                }
            }

            string who = string.Format("Client IP:{0} | HostName:{1} | IPs:{2} | Location:{3} | Assembly:{4} | Class:{5} | Method:{6}", clientIp, name, string.Join(",", allIPs.ToArray()), System.Reflection.Assembly.GetExecutingAssembly().Location, System.Reflection.Assembly.GetExecutingAssembly().FullName, "Present.Maintenance.Program", method);

            return who;
        }
    }
}
