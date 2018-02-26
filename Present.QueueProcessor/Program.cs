using System;
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
            Business.Process.Queue queue = new Business.Process.Queue(string.Empty, QueueProcessor.Properties.Settings.Default.QueueID, 
                QueueProcessor.Properties.Settings.Default.TopCount, 
                new ConsoleLogger.SimpleLogger() {
                    CanAddError = Present.QueueProcessor.Properties.Settings.Default.CanAddError,
                    CanAddInfo = Present.QueueProcessor.Properties.Settings.Default.CanAddInfo,
                    CanAddWarning = Present.QueueProcessor.Properties.Settings.Default.CanAddWarning
                },
                Business.Process.Vault.ExecType.ApiWorker);

            queue.Run(string.Empty);
            //queue.RunAsync(string.Empty);

            Console.WriteLine("Press any key to exit ...");
            Console.Read();
        }
    }
}
