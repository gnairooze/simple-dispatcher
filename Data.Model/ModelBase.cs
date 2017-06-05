using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Data.Model
{
    public class ModelBase
    {
        #region properties
        [Key]
        public long ID { get; set; }
        [Required]
        [Index(IsUnique = true)]
        public Guid BusinessID { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        [Required]
        public DateTime ModifiedOn { get; set; }
        #endregion
    }
}
