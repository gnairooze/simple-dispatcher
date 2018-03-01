using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLogger
{
    [Table("Logs")]
    internal class DataDbLog
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Module { get; set; }
        [Required]
        [Index(IsUnique = true)]
        public Guid BusinessID { get; set; }
        [Required]
        public Guid Group { get; set; }
        [Required]
        public int Counter { get; set; }
        [Required]
        public byte LogType { get; set; }
        [Required]
        [MaxLength(500)]
        public string Who { get; set; }
        [Required]
        public string What { get; set; }
        [Required]
        public DateTime When { get; set; }
        [Required]
        [MaxLength(50)]
        public string ReferenceName { get; set; }
        [Required]
        [MaxLength(50)]
        public string ReferenceValue { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
    }
}
