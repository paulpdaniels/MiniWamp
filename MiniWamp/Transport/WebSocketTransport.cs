

using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
namespace DapperWare.Transport
{

    public class WebSocketTransport : IWampTransport
    {
        private ClientWebSocket _socket;
        private ISerializer _serializer;
        private CancellationTokenSource source;

        public WebSocketTransport()
        {
            this._socket = new ClientWebSocket();
            this._socket.Options.AddSubProtocol("wamp");
            this.source = new CancellationTokenSource();
        }

        public System.Threading.Tasks.Task ConnectAsync(string url)
        {
            return this._socket.ConnectAsync(new System.Uri(url), source.Token);
        }

        public event System.EventHandler Closed;

        public event System.EventHandler Error;

        public event System.EventHandler<WampMessageEventArgs> Message;


        public async void Send(System.Collections.Generic.IEnumerable<object> message)
        {
            //TODO Write into binary
            using (StringWriter writer = new StringWriter()) {
                _serializer.Serialize(writer, message);
                var s = writer.ToString();
                await this._socket.SendAsync(
                    new ArraySegment<byte>(Encoding.Unicode.GetBytes(s)), 
                    WebSocketMessageType.Text, 
                    true, 
                    source.Token);

            }
        }
    }

}