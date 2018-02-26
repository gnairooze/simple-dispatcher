using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SimpleDispatcher.Present.API.Controllers
{
    public class RequestController : ApiController
    {
        // POST api/request
        public HttpResponseMessage Post(JObject data)
        {
            #region log
            int logCounter = 1;
            Guid logGuid = Guid.NewGuid();
            string inputData = data == null ? string.Empty : data.ToString();

            ILogger.LogModel logModel = new ILogger.LogModel()
            {
                BusinessID = Guid.NewGuid(),
                Counter = logCounter++,
                CreatedOn = DateTime.Now,
                Group = logGuid,
                LogType = ILogger.TypeOfLog.Info,
                ReferenceName = "Class.Method",
                ReferenceValue = "Present.API.Controllers.Post",
                What = "First thing. " + inputData,
                When = DateTime.Now,
                Who = API_Business.GetWho("Present.API.Controllers", "Post")
            };

            API_Business.Logger.Log(logModel);
            #endregion

            HttpResponseMessage result = new HttpResponseMessage();

            #region basic validadtion
            Mora.Common.Method.Parameter.Result<List<string>> basicValidationResult = Validation.ValidateAdd(data);
            //API_Business.Logger.Log()
            if (basicValidationResult.Code != Mora.Common.Vault.Result.RESULT_SUCCEEDED_CODE)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                string validationData = JsonConvert.SerializeObject(basicValidationResult);
                result.Content = new StringContent(validationData);

                #region log
                logModel.BusinessID = Guid.NewGuid();
                logModel.Counter = logCounter++;
                logModel.CreatedOn = DateTime.Now;
                logModel.When = DateTime.Now;
                logModel.LogType = ILogger.TypeOfLog.Warning;
                logModel.What = "Failed basic validation. data: " + inputData + ". validation: " + validationData;

                API_Business.Logger.Log(logModel);
                #endregion

                return result;
            }
            #endregion

            #region add request
            Business.View.Request.AddView modelAddRequest = new Business.View.Request.AddView()
            {
                BusinessID = Guid.Parse(data["businessID"].ToString()),
                Data = data.ToString(),
                Operation = data["operation"].ToString(),
                ReferenceName = data["referenceName"].ToString(),
                ReferenceValue = data["referenceValue"].ToString()
            };

            Mora.Common.Method.Parameter.Result<List<string>> addRequestResult = API_Business.Manager.AddRequest(new Business.View.Request.AddView()
            {
                BusinessID = Guid.Parse(data["businessID"].ToString()),
                Data = data.ToString(),
                Operation = data["operation"].ToString(),
                ReferenceName = data["referenceName"].ToString(),
                ReferenceValue = data["referenceValue"].ToString()
            });

            string logAddRequestResult = JsonConvert.SerializeObject(addRequestResult);
            string logAddRequestView = JsonConvert.SerializeObject(modelAddRequest);

            if (addRequestResult.Code == Mora.Common.Vault.Result.RESULT_SUCCEEDED_CODE)
            {
                result.StatusCode = HttpStatusCode.Accepted;
            }
            else
            {
                result.StatusCode = HttpStatusCode.BadRequest;

                result.Content = new StringContent(logAddRequestResult);
            }
            #endregion

            #region log
            logModel.BusinessID = Guid.NewGuid();
            logModel.Counter = logCounter++;
            logModel.CreatedOn = DateTime.Now;
            logModel.When = DateTime.Now;
            
            if(addRequestResult.Code == Mora.Common.Vault.Result.RESULT_SUCCEEDED_CODE)
            {
                logModel.LogType = ILogger.TypeOfLog.Info;
                logModel.What = "Request added successfully. data: " + inputData + ". addRequestView: " + logAddRequestView;
            }
            else
            {
                logModel.LogType = ILogger.TypeOfLog.Warning;
                logModel.What = "Failed adding request. data: " + inputData + ". addRequestView: " + logAddRequestView + ". addRequestResult: " + logAddRequestResult;
            }

            API_Business.Logger.Log(logModel);
            #endregion

            return result;
        }
    }
}
