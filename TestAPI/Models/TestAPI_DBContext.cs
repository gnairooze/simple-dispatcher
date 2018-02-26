using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TestAPI.Models
{
    public class TestAPI_DbContext:DbContext
    {
        public TestAPI_DbContext() : base("TestDB")
        {

        }

        public DbSet<API_Call> API_Calls { get; set; }
    }
}