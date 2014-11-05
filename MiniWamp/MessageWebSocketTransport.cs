using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace DapperWare
{
    class MessageWebSocketTransport : IWampTransport
    {
        private TaskCompletionSource<bool> _connected;


        public MessageWebSocketTransport()
        {
            this._socket = new MessageWebSocket();

            this._socket.Control.SupportedProtocols.Add("wamp");

            this._socket.MessageReceived += _socket_MessageReceived;
        }

        void _socket_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {

            if (this.Message != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //Get the DataReader
                    using (var dataReader = new JsonTextReader(new StreamReader(ms)))
                    {
                        args.GetDataStream().AsStreamForRead().CopyTo(ms);
                        ms.Position = 0;
                        var parsedMessage = JArray.Load(dataReader);
                        Message(this, new WampMessageEventArgs(parsedMessage));
                    }

                }
            }
        }

        public async System.Threading.Tasks.Task ConnectAsync(string url)
        {
            await this._socket.ConnectAsync(new Uri(url));
        }

        public event EventHandler Closed;

        public async void Send(Newtonsoft.Json.Linq.JToken array)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (JsonWriter writer = new JsonTextWriter(new StreamWriter(ms)))
                {
                    DataWriter dataWriter = new DataWriter(this._socket.OutputStream);


                    var result = array.ToString(Formatting.None);
                    dataWriter.WriteString(result);
                    await dataWriter.StoreAsync();
                }


            }
        }

        public event EventHandler<WampMessageEventArgs> Message;
        private MessageWebSocket _socket;
    }
}
