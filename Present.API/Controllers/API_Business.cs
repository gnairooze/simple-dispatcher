using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SimpleDispatcher.Present.API.Controllers
{
    public class API_Business
    {
        public static Business.Request.Manager Manager = new Business.Request.Manager();

        public static DBLogger.SimpleDbLogger Logger = new DBLogger.SimpleDbLogger() {
            CanAddError = Present.API.Properties.Settings.Default.CanLogError,
            CanAddInfo = Present.API.Properties.Settings.Default.CanLogInfo,
            CanAddWarning = Present.API.Properties.Settings.Default.CanLogWarning
        };

        public static string GetWho(string myClass, string myMethod)
        {
            string name = System.Net.Dns.GetHostName();
            System.Net.IPAddress[] ips = System.Net.Dns.GetHostAddresses(name);

            List<string> allIPs = new List<string>();

            if (ips != null)
            {
                foreach (System.Net.IPAddress ip in ips)
                {
                    allIPs.Add(ip.ToString());
                }
            }

            string who = string.Format("Client IP:{0} | HostName:{1} | IPs:{2} | Location:{3} | Assembly:{4} | Class:{5} | Method:{6}", HttpContext.Current.Request.UserHostAddress, name, string.Join(",", allIPs.ToArray()), System.Reflection.Assembly.GetExecutingAssembly().Location, System.Reflection.Assembly.GetExecutingAssembly().FullName, myClass, myMethod);

            return who;
        }
    }
}