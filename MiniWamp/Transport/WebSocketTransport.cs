

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
        private CancellationTokenSource _cts;

        public WebSocketTransport()
        {
            this._socket = new ClientWebSocket();
            this._socket.Options.AddSubProtocol("wamp");
            this._cts = new CancellationTokenSource();
            this._serializer = new DapperWare.Serialization.JsonSerializer();
        }

        //TODO This value seems arbitrary, should come up with a better buffer size
        private byte[] _buffer = new byte[100000];

        private Task _receiver;

        /// <summary>
        /// Connects to the underlying websocket and begins receiving from it
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task ConnectAsync(string url)
        {
            await this._socket.ConnectAsync(new System.Uri(url), _cts.Token);

            _receiver = Task.Run(async () =>
            {
                int offset = 0, count = 0;
            
                while (this._socket.State == WebSocketState.Open)
            
                {
                var result = await this._socket.ReceiveAsync(new ArraySegment<byte>(_buffer, offset, _buffer.Length - offset), this._cts.Token);

                if (result.Count > 0)
                {
                    count += result.Count;
                    offset += count;
                }

                if (result.EndOfMessage)
                {
                    string message = Encoding.UTF8.GetString(_buffer, 0, count);
                    offset = 0; count = 0;

                    this.OnMessage(this, message);
                }
            
                }
            }, _cts.Token);
        }

        public async void Close()
        {
            await CloseAsync();
        }

        public async Task CloseAsync()
        {
            if (this._socket.State == WebSocketState.Open || this._socket.State == WebSocketState.Connecting)
            {
                await this._socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", this._cts.Token);
            }

            if (this.Closed != null)
            {
                this.Closed(this, EventArgs.Empty);
            }
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
            using (MemoryStream stream = new MemoryStream()) {
                _serializer.Serialize(stream, message);

                await this._socket.SendAsync(
                    new ArraySegment<byte>(stream.ToArray()), 
                    WebSocketMessageType.Text, 
                    true, _cts.Token);

            }
        }
    }

}