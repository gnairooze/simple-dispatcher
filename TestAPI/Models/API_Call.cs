using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TestAPI.Models
{
    public class API_Call
    {
        [Required]
        public Guid ID { get; set; }
        [Required]
        [MaxLength(50)]
        [Index]
        public string Operation { get; set; }
        [Required]
        [MaxLength(50)]
        [Index("Request",Order =1)]
        public string Reference_Name { get; set; }
        [Required]
        [MaxLength(50)]
        [Index("Request",Order =2)]
        public string Reference_Value { get; set; }
        [Required]
        public string Request { get; set; }
        [Required]
        public string Response { get; set; }
        [Required]
        public int ResponseCode { get; set; }
        [Required]
        [Index]
        public DateTime Created { get; set; }
    }
}