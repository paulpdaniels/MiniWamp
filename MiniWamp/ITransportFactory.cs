using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare
{
    public interface ITransportFactory
    {
        /// <summary>
        /// Builds a new WampTransport
        /// </summary>
        /// <returns></returns>
        IWampTransport Create();
    }
}
