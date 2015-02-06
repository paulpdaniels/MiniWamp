using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare
{
    public class WampCallException : WampException
    {
        public WampCallException() { }

        public WampCallException(string reason)
            :base(reason)
        {

        }
    }
}
