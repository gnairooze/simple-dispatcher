using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDispatcher.Test.QueueProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            TestData.Fill();

            //TestData.ResetRequests();

            Console.WriteLine("Press any key to exit ...");
            Console.Read();
        }
    }
}
