using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SimpleDispatcher.Present.API.Controllers
{
    public class SettingController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage ResetMaxQueue()
        {
            HttpResponseMessage result = new HttpResponseMessage();
            result.StatusCode = HttpStatusCode.OK;

            API_Business.Manager.SetMaxQueue();

            return result;
        }
    }
}
