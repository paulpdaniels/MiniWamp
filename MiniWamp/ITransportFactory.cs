using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallWamp
{
    public interface ITransportFactory
    {
        IWampTransport Create();
    }
}
