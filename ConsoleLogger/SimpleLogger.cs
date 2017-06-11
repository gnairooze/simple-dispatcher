using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleLogger
{
    public class SimpleLogger : ILogger.ILog
    {
        public int Counter { get; set; }

        public bool CanAddError { get; set; }
        public bool CanAddWarning { get; set; }
        public bool CanAddInfo { get; set; }

        public void Log(ILogger.Priority logPriority, string who, string what, DateTime when)
        {
            switch (logPriority)
            {
                case ILogger.Priority.Error:
                    if (CanAddError)
                    {
                        Console.WriteLine("{0} - {1} | [{2} - {3}] : {4}", this.Counter++, logPriority, who, when.ToShortTimeString(), what);
                    }
                    break;
                case ILogger.Priority.Warning:
                    if (CanAddWarning)
                    {
                        Console.WriteLine("{0} - {1} | [{2} - {3}] : {4}", this.Counter++, logPriority, who, when.ToShortTimeString(), what);
                    }
                    break;
                case ILogger.Priority.Info:
                    if (CanAddInfo)
                    {
                        Console.WriteLine("{0} - {1} | [{2} - {3}] : {4}", this.Counter++, logPriority, who, when.ToShortTimeString(), what);
                    }
                    break;
            }
            
        }
    }
}
