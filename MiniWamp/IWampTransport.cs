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
        event EventHandler Error;
        event EventHandler<WampMessageEventArgs> Message;

        void Send(Newtonsoft.Json.Linq.JToken array);



    }
}
