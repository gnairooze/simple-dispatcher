using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Data.Model
{
    public class QueueDbContext:DbContext
    {
        public QueueDbContext() : base("DispatcherDB")
        { }

        public DbSet<Request> Request { get; set; }
        public DbSet<OperationSettings> OperationSettings { get; set; }
        public DbSet<Worker> Worker { get; set; }
        public DbSet<BasicSetting> BasicSettings { get; set; }

        public void DeleteRequestsNotProcessed(DateTime runDate, int chunk)
        {
            Database.ExecuteSqlCommand("DeleteRequestsNotProcessed @RunDate, @Chunk", new SqlParameter("RunDate", runDate), new SqlParameter("Chunk", chunk));
        }
        public void DeleteRequestsSucceeded(DateTime runDate, int chunk)
        {
            Database.ExecuteSqlCommand("DeleteRequestsSucceeded @RunDate, @Chunk", new SqlParameter("RunDate", runDate), new SqlParameter("Chunk", chunk));
        }
        public void DeleteRequestsFailed(DateTime runDate, int chunk)
        {
            Database.ExecuteSqlCommand("DeleteRequestsFailed @RunDate, @Chunk", new SqlParameter("RunDate", runDate), new SqlParameter("Chunk", chunk));
        }
        public void DeleteRequestsRetrying(DateTime runDate, int chunk)
        {
            Database.ExecuteSqlCommand("DeleteRequestsRetrying @RunDate, @Chunk", new SqlParameter("RunDate", runDate), new SqlParameter("Chunk", chunk));
        }
        public void DeleteRequestsSkipped(DateTime runDate, int chunk)
        {
            Database.ExecuteSqlCommand("DeleteRequestsSkipped @RunDate, @Chunk", new SqlParameter("RunDate", runDate), new SqlParameter("Chunk", chunk));
        }
        public void DeleteRequestsCancelled(DateTime runDate, int chunk)
        {
            Database.ExecuteSqlCommand("DeleteRequestsCancelled @RunDate, @Chunk", new SqlParameter("RunDate", runDate), new SqlParameter("Chunk", chunk));
        }
    }
}
