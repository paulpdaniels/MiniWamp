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
        private MessageWebSocket _socket;

        public MessageWebSocketTransport()
        {
            this._socket = new MessageWebSocket();

            this._socket.Control.SupportedProtocols.Add("wamp");

            this._socket.MessageReceived += _socket_MessageReceived;
            this._socket.Closed += _socket_Closed;
        }

        void _socket_Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            this.OnClosed(sender, args);
        }

        protected virtual void OnClosed(object sender, WebSocketClosedEventArgs args)
        {
            if (this.Closed != null)
            {
                Closed(sender, EventArgs.Empty);
            }
        }

        void _socket_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {

            if (this.Message != null)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //Get the DataReader
                        using (var dataReader = new JsonTextReader(new StreamReader(ms)))
                        {
                            args.GetDataStream().AsStreamForRead().CopyTo(ms);
                            ms.Position = 0;
                            dataReader.SupportMultipleContent = true;
                            while (dataReader.Read())
                            {
                                var parsedMessage = JArray.Load(dataReader);
                                Message(this, new WampMessageEventArgs(parsedMessage));
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    var status = Windows.Networking.Sockets.WebSocketError.GetStatus(ex.HResult);
                    if (status != Windows.Web.WebErrorStatus.Unknown)
                        this.OnError(this, status);
                }
            }
        }

        private void OnError(object sender, Windows.Web.WebErrorStatus status)
        {
            if (this.Error != null)
            {
                Error(sender, EventArgs.Empty);
            } 
        }

        public async System.Threading.Tasks.Task ConnectAsync(string url)
        {
            await this._socket.ConnectAsync(new Uri(url));
        }

        public async void Send(Newtonsoft.Json.Linq.JToken array)
        {
                using (JsonWriter writer = new JsonTextWriter(new StreamWriter(new MemoryStream())))
                {
                    using (DataWriter dataWriter = new DataWriter(this._socket.OutputStream))
                    {
                        var result = array.ToString(Formatting.None);
                        dataWriter.WriteString(result);
                        await dataWriter.StoreAsync();
                        dataWriter.DetachStream();
                    }
                }
        }

        public event EventHandler<WampMessageEventArgs> Message;
        public event EventHandler Closed;
        public event EventHandler Error;
        
    }
}
