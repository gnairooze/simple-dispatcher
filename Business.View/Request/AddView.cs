using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.View.Request
{
    public class AddView
    {
        #region properties
        public Guid BusinessID { get; set; }
        public string Operation { get; set; }
        public string ReferenceName { get; set; }
        public string ReferenceValue { get; set; }
        public string Data { get; set; }
        #endregion
    }
}
