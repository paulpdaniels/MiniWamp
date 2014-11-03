using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace SmallWamp
{
    public class WampClient
    {
        /// <summary>
        /// Identical to calling <code>Connect(url, MessageWebSocketTransportFactory.Default)</code>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<WampSession> Connect(string url)
        {
            return Connect(url, MessageWebSocketTransportFactory.Default);
        }

        public static Task<WampSession> Connect(string url, ITransportFactory factory)
        {
            return Connect(url, () => factory.Create());
        }

        public static async Task<WampSession> Connect(string url, Func<IWampTransport> factoryFn)
        {
            var session = new WampSession(factoryFn());

            await session.ConnectAsync(url);

            return session;
        }

    }
}
