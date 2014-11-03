using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallWamp
{
    public interface IWampTransport
    {
        event EventHandler Closed;

        void Send(Newtonsoft.Json.Linq.JToken array);


        event EventHandler<WampMessageEventArgs> Message;

    }
}
