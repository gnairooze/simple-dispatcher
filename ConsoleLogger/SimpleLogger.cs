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

        public void Log(ILogger.Priority logPriority, string who, string what, DateTime when)
        {
            Console.WriteLine("{0} - {1} | [{2} - {3}] : {4}", this.Counter++, logPriority, who, when.ToShortTimeString(), what);
        }
    }
}
