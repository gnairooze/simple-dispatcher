using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.Request
{
    public class Manager
    {
        private static Data.Model.QueueDbContext _DB = new Data.Model.QueueDbContext();

        public Mora.Common.Method.Parameter.Result<List<string>> AddRequest(Business.View.Request.AddView request)
        {
            Mora.Common.Method.Parameter.Result<List<string>> result = new Mora.Common.Method.Parameter.Result<List<string>>();

            Validation.DB = _DB;
            result = Validation.ValidateAdd(request);

            if(result.Code != Mora.Common.Vault.Result.RESULT_SUCCEEDED_CODE)
            {
                return result;
            }


            Data.Model.Request dataRequest = new Data.Model.Request()
            {
                BusinessID = Guid.NewGuid(),
                CreatedOn = DateTime.Now,
                Data = request.Data,
                ModifiedOn = DateTime.Now,
                Operation = request.Operation,
                OperationSettings_ID = 1
            };
            return result;
        }

        
    }
}
