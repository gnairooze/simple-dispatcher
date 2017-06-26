using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDispatcher.Business.View.Request;
using Mora.Logger.ILogger;

namespace SimpleDispatcher.Business.Exec.Generic
{
    public class Execution
    {
        public event EventHandler<ExecutionCompletedEventArgs> ExecutionCompleted;

        #region attributes
        protected List<IExecutionWorker> _ExecutionWorkers = new List<IExecutionWorker>();
        protected List<IExecutionWorkerAsync> _ExecutionWorkersAsync = new List<IExecutionWorkerAsync>();

        protected int _Counter;
        protected Guid _Group;
        #endregion

        #region constructors
        public Execution(ILog logger, Data.Model.QueueDbContext db)
        {
            this._Counter = 1;
            this._Group = Guid.NewGuid();

            this.Logger = logger;
            this.DB = db;

            loadExecutionWorkers();
            loadExecutionWorkersAsync();
        }

        protected void Worker_ExecutionCompleted(object sender, ExecutionCompletedEventArgs e)
        {
            //bubble the event up to queue
            OnExecutionCompletion(new ExecutionCompletedEventArgs()
            {
                Request = e.Request,
                Worker = e.Worker,
                Succeeded = e.Succeeded
            });
        }

        public void OnExecutionCompletion(ExecutionCompletedEventArgs e)
        {
            EventHandler<ExecutionCompletedEventArgs> handler = ExecutionCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region properties
        public ILog Logger { protected get; set; }
        public Data.Model.QueueDbContext DB { protected get; set; }
        #endregion

        #region entry methods
        public bool Execute(string clientIP, Business.View.Request.ListView request)
        {
            this._Counter = 1;
            this._Group = Guid.NewGuid();

            string who = getWho(System.Reflection.MethodInfo.GetCurrentMethod().Name, clientIP);

            logInfo(who, string.Format("start Execute of request with ID {0}", request.ID), "Request.ID", request.ID.ToString(), this._Counter++, this._Group);

            var worker = (from workerItem in this._ExecutionWorkers
                          where workerItem.ViewModel.BusinessID == request.Worker_BusinessID
                          select workerItem).Single();

            bool succeeded = worker.Execute(request);

            logInfo(who, string.Format("end Execute of request with ID {0} with status {1}", request.ID, succeeded ? "succeeded" : "failed"), "Request.ID", request.ID.ToString(), this._Counter++, this._Group);

            return succeeded;
        }

        public async Task<bool> ExecuteAsync(string clientIP, Business.View.Request.ListView request)
        {
            this._Counter = 1;
            this._Group = Guid.NewGuid();

            string who = getWho(System.Reflection.MethodInfo.GetCurrentMethod().Name, clientIP);

            logInfo(who, string.Format("start Execute of request with ID {0}", request.ID), "Request.ID", request.ID.ToString(), this._Counter++, this._Group);

            var worker = (from workerItem in this._ExecutionWorkersAsync
                          where workerItem.ViewModel.BusinessID == request.Worker_BusinessID
                          select workerItem).Single();

            worker.ExecutionCompleted += Worker_ExecutionCompleted;

            bool succeeded = await worker.ExecuteAsync(request);

            logInfo(who, string.Format("end Execute of request with ID {0} with status {1}", request.ID, succeeded ? "succeeded" : "failed"), "Request.ID", request.ID.ToString(), this._Counter++, this._Group);

            return succeeded;
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Load execution workers. We can have many types doing many businesses.
        /// </summary>
        protected virtual void loadExecutionWorkers()
        {
            string who = getWho(System.Reflection.MethodInfo.GetCurrentMethod().Name, string.Empty);

            logInfo(who, "start loadExecutionWorkers", string.Empty, string.Empty, this._Counter++, this._Group);

            var worker = new TestExecutionWorker();
            worker.ExecutionCompleted += Worker_ExecutionCompleted;

            this._ExecutionWorkers.Add(worker);

            logInfo(who, "end loadExecutionWorkers", string.Empty, string.Empty, this._Counter++, this._Group);
        }

        protected virtual void loadExecutionWorkersAsync()
        {
            string who = getWho(System.Reflection.MethodInfo.GetCurrentMethod().Name, string.Empty);

            logInfo(who, "start loadExecutionWorkersAsync", string.Empty, string.Empty, this._Counter++, this._Group);

            var worker = new TestExecutionWorkerAsync();
            worker.ExecutionCompleted += Worker_ExecutionCompleted;

            this._ExecutionWorkersAsync.Add(worker);

            logInfo(who, "end loadExecutionWorkersAsync", string.Empty, string.Empty, this._Counter++, this._Group);
        }

        protected Guid logInfo(string who, string what, string refName, string refValue, int counter, Guid group)
        {
            if (string.IsNullOrEmpty(refName))
            {
                refName = "N/A";
            }
            if (string.IsNullOrEmpty(refValue))
            {
                refValue = "N/A";
            }
            LogModel model = new LogModel()
            {
                Counter = counter,
                Group = group,
                LogType = TypeOfLog.Info,
                ReferenceName = refName,
                ReferenceValue = refValue,
                What = what,
                When = DateTime.Now,
                Who = who
            };

            Guid result = Guid.Empty;

            result = this.Logger.Log(model);
            
            return result;
        }
        protected string getWho(string method, string clientIp)
        {
            string name = System.Net.Dns.GetHostName();
            System.Net.IPAddress[] ips = System.Net.Dns.GetHostAddresses(name);

            List<string> allIPs = new List<string>();

            if (ips != null)
            {
                foreach (System.Net.IPAddress ip in ips)
                {
                    allIPs.Add(ip.ToString());
                }
            }

            string who = string.Format("Client IP:{0} | HostName:{1} | IPs:{2} | Location:{3} | Assembly:{4} | Class:{5} | Method:{6}", clientIp, name, string.Join(",", allIPs.ToArray()), System.Reflection.Assembly.GetExecutingAssembly().Location, System.Reflection.Assembly.GetExecutingAssembly().FullName, this.GetType().ToString(), method);

            return who;
        }
        #endregion
    }
}
