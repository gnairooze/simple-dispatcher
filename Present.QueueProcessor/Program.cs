using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Present.QueueProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            DBLogger.SimpleDbLogger logger = new DBLogger.SimpleDbLogger()
            {
                CanAddError = Present.QueueProcessor.Properties.Settings.Default.CanAddError,
                CanAddInfo = Present.QueueProcessor.Properties.Settings.Default.CanAddInfo,
                CanAddWarning = Present.QueueProcessor.Properties.Settings.Default.CanAddWarning
            };

            try
            {
                Business.Process.Queue queue = new Business.Process.Queue(Present.QueueProcessor.Properties.Settings.Default.Module, string.Empty, QueueProcessor.Properties.Settings.Default.QueueID, QueueProcessor.Properties.Settings.Default.TopCount, logger, Business.Process.Vault.ExecType.ApiWorker);

                queue.Run(string.Empty);
                //queue.RunAsync(string.Empty);
            }
            catch (Exception ex)
            {
                var id = Guid.NewGuid();
                StringBuilder bld = new StringBuilder();

                logger.Log(new ILogger.LogModel()
                {
                    BusinessID = id,
                    Counter = -1,
                    CreatedOn = DateTime.Now,
                    Group = id,
                    LogType = ILogger.TypeOfLog.Error,
                    Module = Present.QueueProcessor.Properties.Settings.Default.Module,
                    ReferenceName = "Tool",
                    ReferenceValue = Present.QueueProcessor.Properties.Settings.Default.Module,
                    When = DateTime.Now,
                    Who = getWho(),
                    What = extractError(ex, ref bld)
                });
            }
            

            Console.WriteLine("Press any key to exit ...");
            Console.Read();
        }

        private static string extractError(Exception ex, ref StringBuilder bld)
        {
            if (ex.Data != null && ex.Data.Count > 0)
            {
                foreach (DictionaryEntry item in ex.Data)
                {
                    bld.AppendLine(String.Format("Data Key:{0}. Data Value:{1}.", item.Key, item.Value));
                }
            }

            bld.AppendLine(String.Format("Message:{0}.", ex.Message));
            bld.AppendLine(String.Format("StackTrace:{0}.", ex.StackTrace));

            if (ex.InnerException != null)
            {
                extractError(ex.InnerException, ref bld);
            }

            return bld.ToString();
        }

        private static string getWho()
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

            string who = string.Format("HostName:{0} | IPs:{1} | Location:{2} | Assembly:{3} | Class:{4} | Method:{5}", name, string.Join(",", allIPs.ToArray()), System.Reflection.Assembly.GetExecutingAssembly().Location, System.Reflection.Assembly.GetExecutingAssembly().FullName, "Present.QueueProcessor.Program", "Main");

            return who;
        }

    }
}
