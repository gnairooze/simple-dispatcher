using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLogger
{
    internal class LogDbContext:DbContext
    {
        public LogDbContext() : base("LogDb")
        {

        }
        
        public DbSet<DataDbLog> Logs { get; set; }
    }
}
