using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.View.Request
{
    public class ListView
    {
        #region attributes
        protected byte _Status = 0;
        #endregion

        #region constructors
        public ListView()
        {

        }

        public ListView(Data.Model.Request request)
        {
            this.BusinessID = request.BusinessID;
            this.CreatedOn = request.CreatedOn;
            this.Data = request.Data;
            this.ID = request.ID;
            this.ModifiedOn = request.ModifiedOn;
            this.NextRetryOn = request.NextRetryOn;
            this.Operation = request.Operation;
            this.OperationSettings_ID = request.OperationSettings_ID;
            this.QueueID = request.QueueID;
            this.ReferenceName = request.ReferenceName;
            this.ReferenceValue = request.ReferenceValue;
            this.RemainingRetrials = request.RemainingRetrials;
            this.Worker_ID = request.Worker_ID;
            this.Worker_BusinessID = request.Worker_BusinessID;
            this._Status = request.Status;
        }
        #endregion

        #region properties
        public long ID { get; set; }
        public Guid BusinessID { get; set; }
        public long OperationSettings_ID { get; set; }
        public string Operation { get; set; }
        public string ReferenceName { get; set; }
        public string ReferenceValue { get; set; }
        public string Data { get; set; }
        public long Worker_ID { get; set; }
        public Guid Worker_BusinessID { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Vault.RequestStatus Status
        {
            get
            {
                return (Vault.RequestStatus)this._Status;
            }
            set
            {
                this._Status = (byte)value;
            }
        }
        public int RemainingRetrials { get; set; }
        public DateTime? NextRetryOn { get; set; }
        public int QueueID { get; set; }
        #endregion

        public void CopyDataTo(Data.Model.Request request)
        {
            request.NextRetryOn = this.NextRetryOn;
            request.RemainingRetrials = this.RemainingRetrials;
            request.Status = this._Status;
        }
        public void CopyTo(Data.Model.Request request)
        {
            request.BusinessID = this.BusinessID;
            request.Data = this.Data;
            request.NextRetryOn = this.NextRetryOn;
            request.Operation = this.Operation;
            request.OperationSettings_ID = this.OperationSettings_ID;
            request.QueueID = this.QueueID;
            request.ReferenceName = this.ReferenceName;
            request.ReferenceValue = this.ReferenceValue;
            request.RemainingRetrials = this.RemainingRetrials;
            request.Status = this._Status;
            request.Worker_BusinessID = this.Worker_BusinessID;
            request.Worker_ID = this.Worker_ID;
        }
    }

}
