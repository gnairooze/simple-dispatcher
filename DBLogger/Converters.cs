using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLogger
{
    internal class Converters
    {
        public static DataDbLog Convert(ILogger.LogModel model)
        {
            DataDbLog log = new DataDbLog()
            {
                BusinessID = model.BusinessID,
                Counter = model.Counter,
                CreatedOn = model.CreatedOn,
                Group = model.Group,
                ID = model.ID,
                LogType = (byte)model.LogType,
                ReferenceName = model.ReferenceName,
                ReferenceValue = model.ReferenceValue,
                What = model.What,
                When = model.When,
                Who = model.Who
            };

            return log;
        }
    }
}
