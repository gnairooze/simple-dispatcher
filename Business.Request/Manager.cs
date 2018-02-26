using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleDispatcher.Business.View.Request;
using SimpleDispatcher.Data.Model;
using System.Data.Entity.Validation;

namespace SimpleDispatcher.Business.Request
{
    public class Manager
    {
        #region attributes
        private Data.Model.QueueDbContext _DB = new Data.Model.QueueDbContext();
        private Converters _Converters = new Converters();
        private OperationWorkers _OperationWorkers;
        private int _Current_Queue = 1;
        private int _Max_Queue;
        #endregion

        #region constructors
        public Manager()
        {
            this._OperationWorkers = new OperationWorkers(this._DB);

            SetMaxQueue();
        }

        public void SetMaxQueue()
        {
            this._Max_Queue = int.Parse(this._DB.BasicSettings.Where(s => s.Name == "Max_Queue").Single().Value);
        }
        #endregion

        public Mora.Common.Method.Parameter.Result<List<string>> AddRequest(Business.View.Request.AddView request)
        {
            Mora.Common.Method.Parameter.Result<List<string>> result = new Mora.Common.Method.Parameter.Result<List<string>>();

            Validation.DB = _DB;
            result = Validation.ValidateAdd(request);

            if(result.Code != Mora.Common.Vault.Result.RESULT_SUCCEEDED_CODE)
            {
                return result;
            }

            Data.Model.Request dataRequest = this._Converters.ConvertRequestAddViewToModel(request);

            List<Data.Model.Request> dataRequests = this._OperationWorkers.AssignWorkers(dataRequest);

            foreach (var item in dataRequests)
            {
                item.QueueID = this._Current_Queue; //all the result requests will be assigned to the same queue to respect the order of the request in dispatching to workers
            }

            this._DB.Request.AddRange(dataRequests);
            try
            {
                this._DB.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {

                throw;
            }
            catch(Exception ex)
            {
                throw;
            }
            

            updateQueue();

            return result;
        }

        private void updateQueue()
        {
            this._Current_Queue++;

            if (this._Current_Queue > this._Max_Queue)
            {
                this._Current_Queue = 1;
            }
        }
    }
}
