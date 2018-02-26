using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.Request
{
    internal class OperationWorkers
    {
        private Data.Model.QueueDbContext _DB;

        public OperationWorkers(Data.Model.QueueDbContext db)
        {
            this._DB = db;
        }

        public List<Data.Model.Request> AssignWorkers(Data.Model.Request request)
        {
            List<Data.Model.Request> requests = new List<Data.Model.Request>();

            List<Data.Model.OperationSettings> settings = GetOperationWorkersFromDB(request.Operation);

            foreach (var setting in settings)
            {
                Data.Model.Request resultRequest = new Data.Model.Request()
                {
                    BusinessID = request.BusinessID,
                    Data = request.Data,
                    Operation = request.Operation,
                    ReferenceName = request.ReferenceName,
                    ReferenceValue = request.ReferenceValue,
                    CreatedOn = request.CreatedOn,
                    ModifiedOn = request.ModifiedOn,
                    Status = request.Status,
                    OperationSettings_ID = setting.ID,
                    RemainingRetrials = setting.MaxRetrialCount,
                    Worker_BusinessID = setting.Worker_BusinessID,
                    Worker_ID = setting.Worker_ID
                };

                requests.Add(resultRequest);
            }

            return requests;
        }

        private List<Data.Model.OperationSettings> GetOperationWorkersFromDB(string operation)
        {
            var settings = (from setting in this._DB.OperationSettings
                        where setting.Operation.ToLower() == operation.ToLower()
                        select setting).ToList();

            return settings;
        }
    }
}
