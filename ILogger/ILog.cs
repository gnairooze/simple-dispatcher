using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILogger
{
    public interface ILog
    {
        bool CanAddError { get; set; }
        bool CanAddWarning { get; set; }
        bool CanAddInfo { get; set; }

        void Log(Priority logPriority,string who, string what, DateTime when);
    }

    public enum Priority
    {
        Error,
        Warning,
        Info
    }
}
