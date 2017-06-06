using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Data.Model
{
    public class Worker:ModelBase
    {
        #region properties
        [Required]
        [MaxLength(20)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        [Required]
        [MaxLength(2048)]
        public string URL { get; set; }
        [MaxLength(2048)]
        public string Headers { get; set; }
        /// <summary>
        /// timeout in seconds
        /// </summary>
        [Required]
        public int Timeout { get; set; }
        #endregion
    }
}
