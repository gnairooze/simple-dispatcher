using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Business.Request
{
    internal class Converters
    {
        public Data.Model.Request ConvertRequestAddViewToModel(View.Request.AddView request)
        {
            Data.Model.Request dataRequest = new Data.Model.Request()
            {
                BusinessID = request.BusinessID,
                CreatedOn = DateTime.Now,
                Data = request.Data,
                ModifiedOn = DateTime.Now,
                Operation = request.Operation,
                Status = (byte)Business.View.Vault.RequestStatus.NotProcessed,
                ReferenceName = request.ReferenceName,
                ReferenceValue = request.ReferenceValue
            };

            return dataRequest;
        }
    }
}
