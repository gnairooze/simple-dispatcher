using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILogger
{
    public class LogModel
    {
        public long ID { get; set; }
        public Guid BusinessID { get; set; }
        public Guid Group { get; set; }
        public int Counter { get; set; }
        public TypeOfLog LogType { get; set; }
        public string Who { get; set; }
        public string What { get; set; }
        public DateTime When { get; set; }
        public string ReferenceName { get; set; }
        public string ReferenceValue { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
