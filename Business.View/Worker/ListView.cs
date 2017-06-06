using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.View.Worker
{
    public class ListView
    {
        #region constructors
        public ListView()
        {

        }

        public ListView(Data.Model.Worker worker)
        {
            this.BusinessID = worker.BusinessID;
            this.CreatedOn = worker.CreatedOn;
            this.Headers = worker.Headers;
            this.ID = worker.ID;
            this.ModeifiedOn = worker.ModifiedOn;
            this.Name = worker.Name;
            this.Timeout = worker.Timeout;
            this.URL = worker.URL;
        }
        #endregion

        #region properties
        public long ID { get; set; }
        public Guid BusinessID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string Headers { get; set; }
        /// <summary>
        /// timeout in seconds
        /// </summary>
        public int Timeout { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModeifiedOn { get; set; }
        #endregion
    }
}
