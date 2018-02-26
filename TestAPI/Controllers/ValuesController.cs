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
        private Models.TestAPI_DbContext _DbContext = new Models.TestAPI_DbContext();
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
            response.StatusCode = HttpStatusCode.OK;

            this._DbContext.API_Calls.Add(new Models.API_Call()
            {
                Created = DateTime.Now,
                ID = Guid.NewGuid(),
                Operation = data["operation"].ToString(),
                Reference_Name = data["referenceName"].ToString(),
                Reference_Value = data["referenceValue"].ToString(),
                Request = data.ToString(),
                Response = response.ToString(),
                ResponseCode = (int)response.StatusCode
            });

            this._DbContext.SaveChanges();

            return response;
        }
        
    }
}
