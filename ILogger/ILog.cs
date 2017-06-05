using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILogger
{
    public interface ILog
    {
        void Log(Priority logPriority,string who, string what, DateTime when);
    }

    public enum Priority
    {
        Error,
        Warning,
        Info
    }
}
