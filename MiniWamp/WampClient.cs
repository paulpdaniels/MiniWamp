using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace DapperWare
{
    public class WampClient
    {
        /// <summary>
        /// Identical to calling <code>Connect(url, MessageWebSocketTransportFactory.Default)</code>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<WampSession> ConnectAsync(string url)
        {
            return ConnectAsync(url, MessageWebSocketTransportFactory.Default);
        }

        public static Task<WampSession> ConnectAsync(string url, ITransportFactory factory)
        {
            return ConnectAsync(url, () => factory.Create());
        }

        public static async Task<WampSession> ConnectAsync(string url, Func<IWampTransport> factoryFn)
        {
            var session = new WampSession(factoryFn());

            await session.ConnectAsync(url);

            return session;
        }

    }
}
