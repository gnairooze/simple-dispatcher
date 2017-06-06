using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDispatcher.Business.View.Request;
using SimpleDispatcher.Business.Exec.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SimpleDispatcher.Business.Exec.API
{
    public class API_ExecutionWorker : IExecutionWorker
    {
        #region properties
        public View.Worker.ListView ViewModel { get; set; }
        #endregion

        public bool Execute(ListView request)
        {
            Task<HttpResponseMessage> task = post(this.ViewModel.URL, this.ViewModel.Headers, request.Data);

            task.Wait();

            HttpResponseMessage response = task.Result;

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        private async Task<HttpResponseMessage> post(string url, string headers, string data)
        {
            HttpResponseMessage response = null;

            using (var client = new HttpClient())
            {
                #region create http client object
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                foreach (var header in headers.Split(Environment.NewLine.ToCharArray()))
                {
                    string[] splittedHeader = header.Split(":".ToCharArray());
                    client.DefaultRequestHeaders.Add(splittedHeader[0], splittedHeader[1]);
                }
                #endregion

                response = await client.PostAsync(url, new StringContent(data));
            }

            return response;
        }
    }
}
