using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SimpleDispatcher.Present.API.Controllers
{
    public class RequestController : ApiController
    {
        // POST api/request
        public HttpResponseMessage Post(JObject data)
        {
            HttpResponseMessage result = new HttpResponseMessage();

            Mora.Common.Method.Parameter.Result<List<string>> addRequestResult = API_Business.Manager.AddRequest(new Business.View.Request.AddView()
            {
                BusinessID = Guid.Parse(data["businessID"].ToString()),
                Data = data.ToString(),
                Operation = data["operation"].ToString(),
                ReferenceName = data["referenceName"].ToString(),
                ReferenceValue = data["referenceValue"].ToString()
            });

            if (addRequestResult.Code != Mora.Common.Vault.Result.RESULT_SUCCEEDED_CODE)
            {
                result.StatusCode = HttpStatusCode.BadRequest;

                result.Content = new StringContent(JsonConvert.SerializeObject(addRequestResult));
            }
            else
            {
                result.StatusCode = HttpStatusCode.Accepted;
            }

            return result;
        }
    }
}
