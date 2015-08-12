using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare.Session
{
    public class WampConnection
    {
        //Only one session can be active at any one time
        private IWampSession _session;

        /// <summary>
        /// Gets the Url endpoint for this connection
        /// </summary>
        public string Endpoint { get; private set; }

        /// <summary>
        /// The underlying transport for this API
        /// </summary>
        public IWampTransport Transport { get; private set; }

        /// <summary>
        /// The protocol for establishing a session over this transport
        /// </summary>
        public IHandshake Handshake { get; private set; }

        /// <summary>
        /// Builds a new WampConnection
        /// </summary>
        /// <param name="url"></param>
        public WampConnection(string url, IWampTransport transport)
        {
            this.Endpoint = url;
            this.Transport = transport;
            this.Handshake = new SimpleHandshake(this.Transport);
        }

        public async Task<IWampSession> Open()
        {
            //TODO This is not thread safe
            if (_session == null)
            {
                //Handle the client<->server negotiation
                var handshake = Handshake.Open();

                await Transport.ConnectAsync(this.Endpoint);

                var sessionid = await handshake;

                _session = new WampSession(sessionid, Transport);
            }

            return _session;
        }


        private class SimpleHandshake : IHandshake
        {
            private TaskCompletionSource<string> _tcs;

            public SimpleHandshake(IWampTransport transport)
            {
                this.Transport = transport;
            }

            public IWampTransport Transport
            {
                get;
                private set;

            }

            public Task<string> Open()
            {
                _tcs = new TaskCompletionSource<string>();
                this.Transport.Message += Transport_Message;
                return _tcs.Task;
            }

            private void Transport_Message(object sender, WampMessageEventArgs e)
            {
                var type = (MessageType)e.Message[0].Value<int>();

                if (type == MessageType.WELCOME)
                {
                    _tcs.TrySetResult(e.Message[1].Value<string>());
                }
                else
                {
                    _tcs.TrySetException(new Exception("Handshake Failed!"));
                }

                //We are done don't need this anymore
                this.Transport.Message -= Transport_Message;
            }

            public Task Close()
            {
                throw new NotImplementedException();
            }
        }
    }
}
