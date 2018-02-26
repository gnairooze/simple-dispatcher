using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleDispatcher.Present.API.Controllers
{
    internal class Validation
    {
        public static Mora.Common.Method.Parameter.Result<List<string>> ValidateAdd(JObject data)
        {
            Mora.Common.Method.Parameter.Result<List<string>> result = new Mora.Common.Method.Parameter.Result<List<string>>() {
                BusinessID = Guid.NewGuid(),
                Code = Mora.Common.Vault.Result.RESULT_SUCCEEDED_CODE,
                Data = new List<string>(),
                Description = Mora.Common.Vault.Result.RESULT_SUCCEEDED_DESCRIPTION,
                ID = 0
            };

            if (data == null)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_TECH_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_TECH_ERROR_DESCRIPTION;
                result.Data.Add("request is null");

                return result;
            }

            #region validate required fields
            if (data["businessID"] == null)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("businessID is missing");
            }
            else if (data["businessID"].ToString().Trim() == string.Empty)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("businessID is empty");
            }

            if (data["operation"] == null)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("operation is missing");
            }
            else if (data["operation"].ToString().Trim() == string.Empty)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("operation is empty");
            }

            if (data["referenceName"] == null)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("referenceName is missing");
            }
            else if (data["referenceName"].ToString().Trim() == string.Empty)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("referenceName is empty");
            }

            if (data["referenceValue"] == null)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("referenceValue is missing");
            }
            else if (data["referenceValue"].ToString().Trim() == string.Empty)
            {
                result.Code = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_CODE;
                result.Description = Mora.Common.Vault.Result.RESULT_GENERAL_BUSINESS_ERROR_DESCRIPTION;
                result.Data.Add("referenceValue is empty");
            }
            #endregion

            return result;
        }
    }
}