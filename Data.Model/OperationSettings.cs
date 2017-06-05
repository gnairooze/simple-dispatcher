using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Data.Model
{
    public class OperationSettings:ModelBase
    {
        #region properties
        [Required]
        [MaxLength(20)]
        [Index("Operation_Worker_ID",IsUnique = true, Order = 1)]
        public string Operation { get; set; }
        [Required]
        [Index("Operation_Worker_ID", IsUnique = true, Order = 2)]
        public long Worker_ID { get; set; }
        [Required]
        public Guid Worker_BusinessID { get; set; }
        [Required]
        public int MaxRetrialCount { get; set; }
        /// <summary>
        /// retrial delay in seconds
        /// </summary>
        [Required]
        public int RetrialDelay { get; set; }
        /// <summary>
        /// keep succeeded requests duration in days
        /// </summary>
        [Required]
        public int KeepSucceededRequestsDuration { get; set; }
        /// <summary>
        /// keep not processed requests duration in days
        /// </summary>
        [Required]
        public int KeepNotProcessedRequestsDuration { get; set; }
        /// <summary>
        /// keep failed requests duration in days
        /// </summary>
        [Required]
        public int KeepFailedRequestsDuration { get; set; }
        /// <summary>
        /// keep retrying requests duration in days
        /// </summary>
        [Required]
        public int KeepRetryingRequestsDuration { get; set; }
        /// <summary>
        /// keep skipped requests duration in days
        /// </summary>
        [Required]
        public int KeepSkippedRequestsDuration { get; set; }
        /// <summary>
        /// keep cancelled requests duration in days
        /// </summary>
        [Required]
        public int KeepCancelledRequestsDuration { get; set; }
        #endregion
    }
}
