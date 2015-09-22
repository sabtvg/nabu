using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nabu
{
    public class appException : Exception
    {
        public appException(string message):base(message)
        {           
        }
    }
}