using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare.Transport
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

        public IWampTransport Create()
        {
            var transport = new MessageWebSocketTransport();

            return transport;
        }
    }
}
