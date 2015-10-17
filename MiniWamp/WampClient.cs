using DapperWare.Session;
using DapperWare.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare
{
    public class WampClient
    {
        /// <summary>
        /// Identical to calling <code>Connect(url, MessageWebSocketTransportFactory.Default)</code>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<IWampSession> ConnectAsync(string url)
        {
            return ConnectAsync(url, DefaultTransportFactory.Default);
        }

        /// <summary>
        /// Creates a new WampSession by invoking the create method on the provided factory then asynchronously connects
        /// Equivalent to:<br>
        /// <code>
        ///     var session = new WampSession(factory.Create());
        ///     await session.ConnectAsync(url);
        /// </code>
        /// </br>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static Task<IWampSession> ConnectAsync(string url, ITransportFactory factory)
        {
            return ConnectAsync(url, () => factory.Create());
        }

        /// <summary>
        /// Creates a new WampSession by invoking the provided factory function then asynchronously connects
        /// </summary>
        /// <param name="url"></param>
        /// <param name="factoryFn"></param>
        /// <returns></returns>
        public static async Task<IWampSession> ConnectAsync(string url, Func<IWampTransport> factoryFn)
        {
            var connection = new WampConnection(url, factoryFn());

            //var session = new WampSession(factoryFn());

            //await session.ConnectAsync(url);

            return await connection.Open();
        }

    }
}


