using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Networking.Sockets;

namespace SmallWamp
{
    class MessageWebSocketTransport : IWampTransport
    {
        public MessageWebSocketTransport()
        {
            this._socket = new MessageWebSocket();

            this._socket.MessageReceived += _socket_MessageReceived;
        }

        void _socket_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {


            if (this.Message != null)
            {
                //Get the DataReader
                var dataReader = args.GetDataReader();

                //Read the data off the reader
                var message = dataReader.ReadString(dataReader.UnconsumedBufferLength);

                var parsedMessage = JArray.Parse(message);

                Message(this, new WampMessageEventArgs(parsedMessage));
            }
        }

        internal async System.Threading.Tasks.Task ConnectAsync(string url)
        {
            await this._socket.ConnectAsync(new Uri(url));
        }

        public event EventHandler Closed;

        public void Send(Newtonsoft.Json.Linq.JToken array)
        {
            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(this._socket.OutputStream.AsStreamForWrite())))
            {
                array.WriteTo(writer);
            }
        }

        public event EventHandler<WampMessageEventArgs> Message;
        private MessageWebSocket _socket;
    }
}
