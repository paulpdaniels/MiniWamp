using DapperWare.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare
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
            var transport = new WebSocketTransport();

            return transport;
        }
    }
}
