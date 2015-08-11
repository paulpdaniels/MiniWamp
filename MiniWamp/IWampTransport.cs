using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare
{
    public interface IWampTransport
    {
        /// <summary>
        /// Connects to the transport asynchronously
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task ConnectAsync(string url);

        /// <summary>
        /// Closes the underlying transport gracefully
        /// </summary>
        void Close();

        /// <summary>
        /// Raised when the transport is shutting down
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Raised when the transport has errored
        /// </summary>
        event EventHandler Error;

        /// <summary>
        /// Raised when there is a new message from the transport
        /// </summary>
        event EventHandler<WampMessageEventArgs> Message;

        /// <summary>
        /// Sends data across the transport
        /// </summary>
        /// <param name="array"></param>
        //void Send(Newtonsoft.Json.Linq.JToken array);

        void Send(IEnumerable<object> message);


    }
}
