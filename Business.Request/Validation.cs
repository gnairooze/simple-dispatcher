using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.Request
{
    internal class Validation
    {
        public static Data.Model.QueueDbContext DB { get; set; }
        public static Mora.Common.Method.Parameter.Result<List<string>> ValidateAdd(Business.View.Request.AddView request)
        {
            Mora.Common.Method.Parameter.Result<List<string>> result = new Mora.Common.Method.Parameter.Result<List<string>>() {
                BusinessID = Guid.NewGuid(),
                Code = Mora.Common.Vault.Result.RESULT_SUCCEEDED_CODE,
                Data = new List<string>(),
                Description = Mora.Common.Vault.Result.RESULT_SUCCEEDED_DESCRIPTION,
                ID = 0
            };

            if (result == null)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_TECH_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_TECH_ERROR_DESCRIPTION;
                result.Data.Add("request is null");

                return result;
            }

            #region validate required fields
            if (request.BusinessID == Guid.Empty)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("BusinessID is empty");
            }
            else if (string.IsNullOrWhiteSpace(request.Operation))
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("Operation is empty");
            }
            else if (string.IsNullOrWhiteSpace(request.ReferenceName))
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("ReferenceName is empty");
            }
            else if (string.IsNullOrWhiteSpace(request.ReferenceValue))
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("ReferenceValue is empty");
            }
            #endregion

            #region validate operation existed
            bool operationSettingsExisted = DB.OperationSettings.Any(o => o.Operation.ToLower() == request.Operation.ToLower());
            if (!operationSettingsExisted)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("This operation has not been defined in DB");
            }
            #endregion

             #region validate operation existed
            bool requestDuplicated = DB.Request.Any(r => r.BusinessID == request.BusinessID && r.Operation.ToLower() == request.Operation.ToLower());
            if (requestDuplicated)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("This request already added to DB before");
            }
            #endregion

            return result;
        }
    }
}
