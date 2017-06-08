using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDispatcher.Business.View.Request;
using SimpleDispatcher.Business.Exec.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;

namespace SimpleDispatcher.Business.Exec.API
{
    public class API_ExecutionWorker : IExecutionWorkerAsync
    {
        public event EventHandler<ExecutionCompletedEventArgs> ExecutionCompleted;

        #region properties
        public View.Worker.ListView ViewModel { get; set; }
        #endregion

        public bool Execute(ListView request)
        {
            Task<HttpResponseMessage> task = postAsync(this.ViewModel.URL, this.ViewModel.Headers, this.ViewModel.Timeout, request.Data);

            task.Wait();

            HttpResponseMessage response = task.Result;

            bool result = response.StatusCode == System.Net.HttpStatusCode.OK;

            //trigger the event
            OnExecutionCompletion(new ExecutionCompletedEventArgs() {
                Request = request,
                Succeeded = result,
                Worker = this
            });

            return result;
        }

        public async Task<bool> ExecuteAsync(ListView request)
        {
            HttpResponseMessage response = await postAsync(this.ViewModel.URL, this.ViewModel.Headers, this.ViewModel.Timeout, request.Data);

            bool result = response.StatusCode == System.Net.HttpStatusCode.OK;
            
            //trigger the event
            OnExecutionCompletion(new ExecutionCompletedEventArgs()
            {
                Request = request,
                Succeeded = result,
                Worker = this
            });

            return result;
        }

        private async Task<HttpResponseMessage> postAsync(string url, string headers, int timeoutSeconds, string data)
        {
            HttpResponseMessage response = null;

            using (var client = new HttpClient())
            {
                #region create http client object
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = new TimeSpan(0,0, timeoutSeconds);

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
        public void OnExecutionCompletion(ExecutionCompletedEventArgs e)
        {
            EventHandler<ExecutionCompletedEventArgs> handler = ExecutionCompleted;

            if(handler != null)
            {
                handler(this, e);
            }
        }
    }
}
