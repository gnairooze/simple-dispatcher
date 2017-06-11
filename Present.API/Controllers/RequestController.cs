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

            return result;
        }
    }
}
