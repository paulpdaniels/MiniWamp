using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare
{
    public class WampMessageEventArgs : EventArgs
    {

        public JArray Message { get; private set; }

        public WampMessageEventArgs(JArray message)
        {
            this.Message = message;
        }

    }
}
