using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Security.Cryptography;

namespace DapperWare
{
    public class WampSession
    {
        #region Private Members
        private Dictionary<string, IWampSubject> _topics;
        private Dictionary<string, Action<Exception, JToken>> _pendingCalls;
        private Dictionary<string, string> _prefixes;
        private Dictionary<MessageType, Action<JArray>> _messageHandlers;
        private IWampTransport _transport;
        private TaskCompletionSource<bool> _welcomed; 
        #endregion

        #region Properties
        public IWampTransport Transport
        {
            get
            {
                return this._transport;
            }
        }

        public IEnumerable<IWampSubject> Subscriptions
        {
            get
            {
                return this._topics.Values;
            }
        }

        public IDictionary<string, string> Prefixes
        {
            get
            {
                if (this._prefixes == null)
                {
                    this._prefixes = new Dictionary<string, string>();
                }

                return this._prefixes;
            }
        }

        public string SessionId { get; private set; }
        #endregion

        #region Constructor
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
        #endregion

        #region Public
        public Task<T> Call<T>(string method, params object[] content)
        {
            string call_id = null;

            do
            {
                call_id = GenerateCallId();
            }
            while (this._pendingCalls.ContainsKey(call_id));

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
            WampSubject<T> rootSubject = null;
            topic = this.Resolve(topic);
            if (!this._topics.TryGetValue(topic, out subject))
            {
                this._topics[topic] = rootSubject = new WampSubject<T>(this, topic);
                this._transport.Send(new JArray(5, topic));
            }
            else
            {
                rootSubject = (WampSubject<T>)subject;
            }

            return rootSubject.CreateChild();
        }

        public void Publish<T>(string topic, T ev)
        {
            JArray array = new JArray(MessageType.SUBSCRIBE, topic, ev);

            WriteMessage(array);
        }

        public void Unsubscribe(string topic)
        {
            this._topics.Remove(topic);
            WriteMessage(new JArray(MessageType.UNSUBSCRIBE, topic));
        }
        #endregion

        internal async Task ConnectAsync(string url)
        {
            this._welcomed = new TaskCompletionSource<bool>();

            await this._transport.ConnectAsync(url);

            await this._welcomed.Task;
        }

        private void transport_Message(object sender, WampMessageEventArgs e)
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

            Action<JArray> action;
            if (_messageHandlers.TryGetValue(type, out action))
            {
                action(parsedMessage);
            }
            else
            {
                RaiseException(new WampException());
            }
        }

        protected virtual void RaiseException(WampException wampException)
        {
            throw new NotImplementedException();
        }

        private void WriteMessage(JToken array)
        {
            this._transport.Send(array);
        }

        private string GenerateCallId()
        {
            return CryptographicBuffer.EncodeToBase64String(CryptographicBuffer.GenerateRandom(20));
        }

        private string Resolve(string curie)
        {
            if (Uri.IsWellFormedUriString(curie, UriKind.Absolute))
                return curie;
            else
            {
                var i = curie.IndexOf(":", StringComparison.Ordinal);

                if (i > 0)
                {
                    string prefix = curie.Substring(0, i);
                    string mapped;

                    if (Prefixes.TryGetValue(prefix, out mapped))
                    {
                        return mapped + curie.Substring(i + 1);
                    }
                }
            }

            return curie;
        }

    }
}
