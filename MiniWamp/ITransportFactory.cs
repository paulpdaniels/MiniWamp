using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare
{
    public interface ITransportFactory
    {
        IWampTransport Create();
    }
}
