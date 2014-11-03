using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallWamp
{
    public class MessageWebSocketTransportFactory : ITransportFactory

    {
        private static readonly MessageWebSocketTransportFactory _default = new MessageWebSocketTransportFactory();

        public static MessageWebSocketTransportFactory Default
        {
            get
            {
                return _default;
            }
        }

        public async System.Threading.Tasks.Task<IWampTransport> CreateAsync(string url)
        {
            var transport = new MessageWebSocketTransport();

            await transport.ConnectAsync(url);

            return transport;
        }
    }
}
