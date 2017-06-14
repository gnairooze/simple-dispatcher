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

        Guid Log(LogModel model);
    }

    public enum TypeOfLog
    {
        Error,
        Warning,
        Info
    }
}
