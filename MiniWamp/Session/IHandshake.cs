using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare.Session
{
    public interface IHandshake
    {
        /// <summary>
        /// Gets the transport that this handshake will operate over
        /// </summary>
        IWampTransport Transport { get; }

        /// <summary>
        /// Readies a transport session for use by a WampSession
        /// </summary>
        /// <returns>A task with the session id after negotiation</returns>
        Task<string> Open();

        /// <summary>
        /// Terminates the connection gracefully
        /// </summary>
        /// <returns></returns>
        Task Close();

    }
}
