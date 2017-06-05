using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.View.OperationSettings
{
    public class ListView
    {
        #region constrcutors
        public ListView()
        {

        }
        public ListView(Data.Model.OperationSettings operationSettings)
        {
            this.BusinessID = operationSettings.BusinessID;
            this.CreatedOn = operationSettings.CreatedOn;
            this.ID = operationSettings.ID;
            this.KeepCancelledRequestsDuration = operationSettings.KeepCancelledRequestsDuration;
            this.KeepFailedRequestsDuration = operationSettings.KeepFailedRequestsDuration;
            this.KeepNotProcessedRequestsDuration = operationSettings.KeepNotProcessedRequestsDuration;
            this.KeepRetryingRequestsDuration = operationSettings.KeepRetryingRequestsDuration;
            this.KeepSkippedRequestsDuration = operationSettings.KeepSkippedRequestsDuration;
            this.KeepSucceededRequestsDuration = operationSettings.KeepSucceededRequestsDuration;
            this.MaxRetrialCount = operationSettings.MaxRetrialCount;
            this.ModifiedOn = operationSettings.ModifiedOn;
            this.Operation = operationSettings.Operation;
            this.RetrialDelay = operationSettings.RetrialDelay;
            this.Worker_BusinessID = operationSettings.Worker_BusinessID;
            this.Worker_ID = operationSettings.Worker_ID;
        }
        #endregion

        #region properties
        public long ID { get; set; }
        public Guid BusinessID { get; set; }
        public string Operation { get; set; }
        public long Worker_ID { get; set; }
        public Guid Worker_BusinessID { get; set; }
        public int MaxRetrialCount { get; set; }
        /// <summary>
        /// retrial delay in seconds
        /// </summary>
        public int RetrialDelay { get; set; }
        /// <summary>
        /// keep succeeded requests duration in days
        /// </summary>
        public int KeepSucceededRequestsDuration { get; set; }
        public int KeepNotProcessedRequestsDuration { get; set; }
        /// <summary>
        /// keep failed requests duration in days
        /// </summary>
        public int KeepFailedRequestsDuration { get; set; }
        /// <summary>
        /// keep retrying requests duration in days
        /// </summary>
        public int KeepRetryingRequestsDuration { get; set; }
        /// <summary>
        /// keep skipped requests duration in days
        /// </summary>
        public int KeepSkippedRequestsDuration { get; set; }
        /// <summary>
        /// keep cancelled requests duration in days
        /// </summary>
        public int KeepCancelledRequestsDuration { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        #endregion
    }
}
