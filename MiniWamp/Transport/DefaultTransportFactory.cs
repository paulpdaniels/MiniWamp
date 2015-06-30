using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare.Transport
{
    public class DefaultTransportFactory : ITransportFactory
    {
        private static readonly DefaultTransportFactory _default = new DefaultTransportFactory();

        public static DefaultTransportFactory Default
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
