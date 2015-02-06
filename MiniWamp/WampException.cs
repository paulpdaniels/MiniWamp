using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare
{
    public class WampException : Exception
    {
        public WampException() { }
        public WampException(string reason) : base(reason) { }
    }
}
