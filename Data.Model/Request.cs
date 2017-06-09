using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Data.Model
{
    public class Request:ModelBase
    {
        #region properties
        [Required]
        [Index]
        public long OperationSettings_ID { get; set; }
        [Required]
        [MaxLength(20)]
        [Index]
        public string Operation { get; set; }
        [Required]
        [MaxLength(50)]
        [Index("Reference", Order = 1)]
        public string ReferenceName { get; set; }
        [Required]
        [MaxLength(50)]
        [Index("Reference", Order = 2)]
        public string ReferenceValue { get; set; }
        [Required]
        public string Data { get; set; }
        [Required]
        [Index]
        public long Worker_ID { get; set; }
        [Required]
        public Guid Worker_BusinessID { get; set; }
        [Required]
        public byte Status { get; set; }
        [Required]
        public int RemainingRetrials { get; set; }
        public DateTime? NextRetryOn { get; set; }
        [Required]
        [Index]
        public int QueueID { get; set; }
        #endregion
    }
}
