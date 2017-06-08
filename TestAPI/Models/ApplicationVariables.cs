using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestAPI.Models
{
    internal class ApplicationVariables
    {
        public static int Counter
        {
            get
            {
                int counter = 0;

                if(HttpContext.Current.Application["Counter"] != null)
                {
                    counter = (int)HttpContext.Current.Application["Counter"];
                }
                else
                {
                    HttpContext.Current.Application["Counter"] = counter;
                }

                return counter;
            }
            set
            {
                HttpContext.Current.Application["Counter"] = value;
            }
        }
    }
}