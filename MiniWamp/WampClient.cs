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

        public static async Task<WampSession> Connect(string url)
        {
            return await Connect(url, MessageWebSocketTransportFactory.Default);
        }

        public static async Task<WampSession> Connect(string url, ITransportFactory factory)
        {
            var transport = await factory.CreateAsync(url);

            return new WampSession(transport);
        }

    }
}
