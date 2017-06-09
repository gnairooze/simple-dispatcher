using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Test.QueueProcessor
{
    internal class TestData
    {
        private static Data.Model.QueueDbContext _DB = new Data.Model.QueueDbContext();

        public static void Fill()
        {
            deleteOperationsSettings();

            Guid worker_BusinessID = Guid.Parse("A92DF39B-EEC6-4967-989C-9C3177BE1231");
            long worker_ID = 1;

            fillWorkers(worker_BusinessID, worker_ID);

            fillOperationsSettings(worker_BusinessID, worker_ID);
            fillRequests();
        }

        public static void ResetRequests()
        {
            var requests = TestData._DB.Request;

            foreach (var request in requests)
            {
                request.ModifiedOn = request.CreatedOn;
                request.NextRetryOn = null;
                request.RemainingRetrials = 2;
                request.Status = 1;
            }

            TestData._DB.SaveChanges();
        }

        private static void fillRequests()
        {
            Guid workerGuid = Guid.Parse("A92DF39B-EEC6-4967-989C-9C3177BE1231");

            for (int i = 0; i < 20000; i++)
            {
                TestData._DB.Request.Add(new Data.Model.Request()
                {
                    ID = i,
                    BusinessID = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    Data = "Test Data " + Convert.ToChar(65 + (i % 4)),
                    ModifiedOn = DateTime.Now,
                    NextRetryOn = null,
                    Operation = "Operation " + Convert.ToChar(65 + (i % 4)),
                    QueueID = i % 2,
                    ReferenceName = "Test Reference ID",
                    ReferenceValue = (i % 7000).ToString(),
                    RemainingRetrials = 2,
                    Status = 1,
                    Worker_BusinessID = workerGuid,
                    Worker_ID = 1,
                    OperationSettings_ID = i % 4
                });
            }

            TestData._DB.SaveChanges();
        }

        private static void fillOperationsSettings(Guid worker_BusinessID, long worker_ID)
        {
            for (int i = 0; i < 4; i++)
            {
                TestData._DB.OperationSettings.Add(new Data.Model.OperationSettings()
                {
                    ID = i,
                    BusinessID = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    KeepCancelledRequestsDuration = 1,
                    KeepFailedRequestsDuration = 1,
                    KeepNotProcessedRequestsDuration = 1,
                    KeepRetryingRequestsDuration = 1,
                    KeepSkippedRequestsDuration = 1,
                    KeepSucceededRequestsDuration = 1,
                    MaxRetrialCount = 2,
                    ModifiedOn = DateTime.Now,
                    Operation = "Operation " + Convert.ToChar(65+i),
                    RetrialDelay = 10,
                    Worker_BusinessID = worker_BusinessID,
                    Worker_ID = worker_ID
                });
            }

            TestData._DB.SaveChanges();
            
        }

        private static void deleteOperationsSettings()
        {
            TestData._DB.OperationSettings.RemoveRange(TestData._DB.OperationSettings);

            TestData._DB.SaveChanges();
        }

        private static void fillWorkers(Guid worker_BusinessID, long worker_ID)
        {
            TestData._DB.Worker.Add(new Data.Model.Worker() {
                BusinessID = worker_BusinessID,
                CreatedOn = DateTime.Now,
                Headers = new StringBuilder().AppendLine("content-type:application/json").ToString(),
                ID = worker_ID,
                ModifiedOn = DateTime.Now,
                Name = "TestAPI",
                Timeout = 10,
                URL = "http://localhost:8090/TestAPI/api/values"
            });
        }
    }
}
