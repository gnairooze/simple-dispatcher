using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILogger;

namespace DBLogger
{
    public class SimpleDbLogger : ILogger.ILog
    {
        private LogDbContext _Db = new LogDbContext();
        public int Counter { get; set; }
        public bool CanAddError { get; set; }
        public bool CanAddWarning { get; set; }
        public bool CanAddInfo { get; set; }

        public Guid Log(LogModel model)
        {
            DataDbLog dataModel = Converters.Convert(model);
            dataModel.BusinessID = Guid.NewGuid();

            switch (model.LogType)
            {
                case ILogger.TypeOfLog.Error:
                    if (CanAddError)
                    {
                         Console.WriteLine("{0} - {1} | [{2} - {3}] : {4}", this.Counter++, model.LogType, model.Who, model.When.ToShortTimeString(), model.What);
                        this._Db.Logs.Add(dataModel);
                    }
                    break;
                case ILogger.TypeOfLog.Warning:
                    if (CanAddWarning)
                    {
                         Console.WriteLine("{0} - {1} | [{2} - {3}] : {4}", this.Counter++, model.LogType, model.Who, model.When.ToShortTimeString(), model.What);
                        this._Db.Logs.Add(dataModel);
                    }
                    break;
                case ILogger.TypeOfLog.Info:
                    if (CanAddInfo)
                    {
                         Console.WriteLine("{0} - {1} | [{2} - {3}] : {4}", this.Counter++, model.LogType, model.Who, model.When.ToShortTimeString(), model.What);
                        this._Db.Logs.Add(dataModel);
                    }
                    break;
            }

            this._Db.SaveChanges();

            return dataModel.BusinessID;
        }
    }
}
