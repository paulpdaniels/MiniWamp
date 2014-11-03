using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare
{
    public interface IWampTransport
    {
        System.Threading.Tasks.Task ConnectAsync(string url);

        event EventHandler Closed;

        void Send(Newtonsoft.Json.Linq.JToken array);


        event EventHandler<WampMessageEventArgs> Message;

    }
}
