using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace TestAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values/5
        public string Get()
        {
            return "Test API is up and running";
        }

        // POST api/values
        public HttpResponseMessage Post(JObject data)
        {
            int counter = Models.ApplicationVariables.Counter++;

            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(string.Format("This is a test - {0}", counter));
            Thread.Sleep(5000);

            if(counter % 13 == 0)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
            }

            return response;
        }
        
    }
}
