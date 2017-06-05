using System;
using System.Collections.Generic;
using System.Data.Entity;
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

    }
}
