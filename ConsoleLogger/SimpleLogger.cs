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

        public Guid Log(ILogger.LogModel model)
        {
            Guid result = Guid.Empty;

            switch (model.LogType)
            {
                case ILogger.TypeOfLog.Error:
                    if (CanAddError)
                    {
                        Console.WriteLine("{0} - {1} | [{2} - {3}] : {4}", this.Counter++, model.LogType, model.Who, model.When.ToShortTimeString(), model.What);
                        result = Guid.NewGuid();
                    }
                    break;
                case ILogger.TypeOfLog.Warning:
                    if (CanAddWarning)
                    {
                        Console.WriteLine("{0} - {1} | [{2} - {3}] : {4}", this.Counter++, model.LogType, model.Who, model.When.ToShortTimeString(), model.What);
                        result = Guid.NewGuid();
                    }
                    break;
                case ILogger.TypeOfLog.Info:
                    if (CanAddInfo)
                    {
                        Console.WriteLine("{0} - {1} | [{2} - {3}] : {4}", this.Counter++, model.LogType, model.Who, model.When.ToShortTimeString(), model.What);
                        result = Guid.NewGuid();
                    }
                    break;
            }

            return result;
        }
    }
}
