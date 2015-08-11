

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DapperWare.Transport
{

    public class WebSocketTransport : IWampTransport
    {
        private ClientWebSocket _socket;
        private ISerializer _serializer;
        private CancellationTokenSource cts;

        public WebSocketTransport()
        {
            this._socket = new ClientWebSocket();
            this._socket.Options.AddSubProtocol("wamp");
            this.cts = new CancellationTokenSource();
            this._serializer = new DapperWare.Serialization.JsonSerializer();
        }

        private byte[] buffer = new byte[100000];

        private Task receiver;

        public async System.Threading.Tasks.Task ConnectAsync(string url)
        {
            await this._socket.ConnectAsync(new System.Uri(url), cts.Token);

            receiver = Task.Run(async () =>
            {
                            
                int offset = 0, count = 0;
            
                while (this._socket.State == WebSocketState.Open)
            
                {
                var result = await this._socket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, buffer.Length - offset), this.cts.Token);

                if (result.Count > 0)
                {
                    count += result.Count;
                    offset += count;
                }

                if (result.EndOfMessage)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, count);
                    offset = 0; count = 0;

                    this.OnMessage(this, message);
                }
            
                }
            }, cts.Token);
        }

        protected virtual void OnMessage(object sender, string message)
        {
            var arr = JArray.Parse(message);

            if (Message != null)
            {
                this.Message(sender, new WampMessageEventArgs(arr));
            }
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
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(s)), 
                    WebSocketMessageType.Text, 
                    true, 
                    cts.Token);

            }
        }
    }

}