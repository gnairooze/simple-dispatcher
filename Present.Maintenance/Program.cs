using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Present.Maintenance
{
    class Program
    {
        static Mora.Logger.MsSqlLogger.DbLogger _Logger = new Mora.Logger.MsSqlLogger.DbLogger() {
            CanAddError = Present.Maintenance.Properties.Settings.Default.CanAddError,
            CanAddInfo = Present.Maintenance.Properties.Settings.Default.CanAddInfo,
            CanAddWarning = Present.Maintenance.Properties.Settings.Default.CanAddWarning
        };

        static int _Counter;
        static Guid _Group;

        static void Main(string[] args)
        {
            _Counter = 1;
            _Group = Guid.NewGuid();

            DateTime runDate = DateTime.Now;
            string who = getWho(System.Reflection.MethodInfo.GetCurrentMethod().Name, string.Empty);

            logInfo(who, string.Format("EnableDeleteCancelled: {0}", Present.Maintenance.Properties.Settings.Default.EnableDeleteCancelled), string.Empty, string.Empty, _Counter++, _Group);
            if (Present.Maintenance.Properties.Settings.Default.EnableDeleteCancelled)
            {
                logInfo(who, "start DeleteCancelledRequests", string.Empty, string.Empty, _Counter++, _Group);
                Business.Maintenance.Manager.DeleteCancelledRequests(runDate, Present.Maintenance.Properties.Settings.Default.DeleteChunk, Present.Maintenance.Properties.Settings.Default.DeleteTimeout);
                logInfo(who, "end DeleteCancelledRequests", string.Empty, string.Empty, _Counter++, _Group);
            }

            logInfo(who, string.Format("EnableDeleteFailed: {0}", Present.Maintenance.Properties.Settings.Default.EnableDeleteFailed), string.Empty, string.Empty, _Counter++, _Group);
            if (Present.Maintenance.Properties.Settings.Default.EnableDeleteFailed)
            {
                logInfo(who, "start DeleteFailedRequests", string.Empty, string.Empty, _Counter++, _Group);
                Business.Maintenance.Manager.DeleteFailedRequests(runDate, Present.Maintenance.Properties.Settings.Default.DeleteChunk, Present.Maintenance.Properties.Settings.Default.DeleteTimeout);
                logInfo(who, "end DeleteFailedRequests", string.Empty, string.Empty, _Counter++, _Group);
            }

            logInfo(who, string.Format("EnableDeleteNotProcessed: {0}", Present.Maintenance.Properties.Settings.Default.EnableDeleteNotProcessed), string.Empty, string.Empty, _Counter++, _Group);
            if (Present.Maintenance.Properties.Settings.Default.EnableDeleteNotProcessed)
            {
                logInfo(who, "start DeleteNotProcessedRequests", string.Empty, string.Empty, _Counter++, _Group);
                Business.Maintenance.Manager.DeleteNotProcessedRequests(runDate, Present.Maintenance.Properties.Settings.Default.DeleteChunk, Present.Maintenance.Properties.Settings.Default.DeleteTimeout);
                logInfo(who, "end DeleteNotProcessedRequests", string.Empty, string.Empty, _Counter++, _Group);
            }

            logInfo(who, string.Format("EnableDeleteRetrying: {0}", Present.Maintenance.Properties.Settings.Default.EnableDeleteRetrying), string.Empty, string.Empty, _Counter++, _Group);
            if (Present.Maintenance.Properties.Settings.Default.EnableDeleteRetrying)
            {
                logInfo(who, "start DeleteRetryingRequests", string.Empty, string.Empty, _Counter++, _Group);
                Business.Maintenance.Manager.DeleteRetryingRequests(runDate, Present.Maintenance.Properties.Settings.Default.DeleteChunk, Present.Maintenance.Properties.Settings.Default.DeleteTimeout);
                logInfo(who, "end DeleteRetryingRequests", string.Empty, string.Empty, _Counter++, _Group);;
            }

            logInfo(who, string.Format("EnableDeleteSkipped: {0}", Present.Maintenance.Properties.Settings.Default.EnableDeleteSkipped), string.Empty, string.Empty, _Counter++, _Group);
            if (Present.Maintenance.Properties.Settings.Default.EnableDeleteSkipped)
            {
                logInfo(who, "start DeleteSkippedRequests", string.Empty, string.Empty, _Counter++, _Group);
                Business.Maintenance.Manager.DeleteSkippedRequests(runDate, Present.Maintenance.Properties.Settings.Default.DeleteChunk, Present.Maintenance.Properties.Settings.Default.DeleteTimeout);
                logInfo(who, "start DeleteSkippedRequests", string.Empty, string.Empty, _Counter++, _Group);
            }

            logInfo(who, string.Format("EnableDeleteSucceeded: {0}", Present.Maintenance.Properties.Settings.Default.EnableDeleteSucceeded), string.Empty, string.Empty, _Counter++, _Group);
            if (Present.Maintenance.Properties.Settings.Default.EnableDeleteSucceeded)
            {
                logInfo(who, "start DeleteSucceededRequests", string.Empty, string.Empty, _Counter++, _Group);
                Business.Maintenance.Manager.DeleteSucceededRequests(runDate, Present.Maintenance.Properties.Settings.Default.DeleteChunk, Present.Maintenance.Properties.Settings.Default.DeleteTimeout);
                logInfo(who, "end DeleteSucceededRequests", string.Empty, string.Empty, _Counter++, _Group);
            }
        }

        private static Guid logInfo(string who, string what, string refName, string refValue, int counter, Guid group)
        {
            Mora.Logger.ILogger.LogModel model = new Mora.Logger.ILogger.LogModel()
            {
                Counter = counter,
                Group = group,
                LogType = Mora.Logger.ILogger.TypeOfLog.Info,
                ReferenceName = refName,
                ReferenceValue = refValue,
                What = what,
                When = DateTime.Now,
                Who = who
            };

            return _Logger.Log(model);
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

            string who = string.Format("Client IP:{0} | HostName:{1} | IPs:{2} | Location:{3} | Assembly:{4} | Class:{5} | Method:{6}", clientIp, name, string.Join(",", allIPs.ToArray()), System.Reflection.Assembly.GetExecutingAssembly().Location, System.Reflection.Assembly.GetExecutingAssembly().FullName, "SimpleDispatcher.Present.Maintenance.Program", method);

            return who;
        }
    }
}
