using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace DapperWare
{
    public class WampSession
    {
        private static Random sRandom = new Random();

        private Dictionary<string, IWampSubject> _topics;
        private Dictionary<string, Action<Exception, JToken>> _pendingCalls;
        private Dictionary<MessageType, Action<JArray>> _messageHandlers;
        private IWampTransport _transport;
        private TaskCompletionSource<bool> _welcomed;


        public IWampTransport Transport
        {
            get
            {
                return this._transport;
            }
        }

        public WampSession(IWampTransport transport)
        {
            this._pendingCalls = new Dictionary<string, Action<Exception, JToken>>();
            this._messageHandlers = new Dictionary<MessageType, Action<JArray>>();
            this._topics = new Dictionary<string, IWampSubject>();

            this._transport = transport;
            this._transport.Message += transport_Message;

            this._messageHandlers[MessageType.WELCOME] = OnWelcome;
            this._messageHandlers[MessageType.CALLRESULT] = OnCallResult;
            this._messageHandlers[MessageType.CALLERROR] = OnCallError;
            this._messageHandlers[MessageType.EVENT] = OnEvent;
        }

       

        void transport_Message(object sender, WampMessageEventArgs e)
        {
            var type = (MessageType)e.Message[0].Value<int>();

            _messageHandlers[type](e.Message);   
        }

        private void OnWelcome(JArray obj)
        {
            if (this._welcomed != null)
            {
                this.SessionId = obj[1].Value<string>();
                this._welcomed.SetResult(true);
            }
        }

        private void OnCallResult(JArray m)
        {
            var call_id = m[1].Value<string>();

            this._pendingCalls[call_id](null, m[2]);
        }

        private void OnEvent(JArray m)
        {
            var topic = m[1].Value<string>();

            IWampSubject subject = null;

            if (this._topics.TryGetValue(topic, out subject))
            {
                subject.HandleEvent(topic, m[2]);
            }
        }

        private void OnCallError(JArray m)
        {
            var call_id = m[1].Value<string>();

            var exception = new WampCallException("Error on on call to topic: " + m[2].Value<string>());

            this._pendingCalls[call_id](exception, default(JToken));
        }

        void socket_MessageReceived(Windows.Networking.Sockets.MessageWebSocket sender, Windows.Networking.Sockets.MessageWebSocketMessageReceivedEventArgs args)
        {
            //Get the DataReader
            var dataReader = args.GetDataReader();

            //Read the data off the reader
            var message = dataReader.ReadString(dataReader.UnconsumedBufferLength);

            var parsedMessage = JArray.Parse(message);

            var type = (MessageType)parsedMessage[0].Value<int>();

            _messageHandlers[type](parsedMessage);
        }

        public Task<T> Call<T>(string method, params object[] content)
        {
            string call_id = null;

            do {
                call_id = GenerateCallId(); 
            }
            while(this._pendingCalls.ContainsKey(call_id));

            List<object> arr = new List<object> { 2, call_id, method };

            foreach (var item in content)
            {
                arr.Add(item);
            }

            TaskCompletionSource<T> source = new TaskCompletionSource<T>();

            this._pendingCalls[call_id] = (ex, t) =>
            {
                if (ex != null)
                {
                    source.SetException(ex);
                    return;
                }

                source.SetResult(t.ToObject<T>());
            };

            this._transport.Send(JArray.FromObject(arr));

            return source.Task;

        }

        public IWampSubject<T> Subscribe<T>(string topic)
        {
            IWampSubject subject = null;

            if (!this._topics.TryGetValue(topic, out subject))
            {
                subject = this._topics[topic] = new WampSubject<T>(this, topic);

                this._transport.Send(new JArray(5, topic));
            }

            return (IWampSubject<T>)subject;
        }

        public void Publish<T>(string topic, T ev)
        {
            JArray array = new JArray(MessageType.SUBSCRIBE, topic, ev);

            WriteMessage(array);
        }

        public IEnumerable<IWampSubject> Subscriptions
        {
            get
            {
                return this._topics.Values;
            }
        }

        private void WriteMessage(JToken array)
        {
            this._transport.Send(array);
        }

        private string GenerateCallId()
        {
            byte[] buffer = new byte[20];
            sRandom.NextBytes(buffer);
            return System.Text.Encoding.UTF8.GetString(buffer, 0, 20);
        }

        public void Unsubscribe(string key)
        {
            this._topics.Remove(key);
        }

        public string SessionId { get; private set; }

        internal async Task ConnectAsync(string url)
        {
            this._welcomed = new TaskCompletionSource<bool>();

            await this._transport.ConnectAsync(url);

            await this._welcomed.Task;
        }
    }
}
