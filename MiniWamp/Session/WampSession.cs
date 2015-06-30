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
        private Dictionary<string, IWampSubscription> _topics;
        private Dictionary<string, Action<Exception, JToken>> _pendingCalls;
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

        public IEnumerable<IWampSubscription> Subscriptions
        {
            get
            {
                return this._topics.Values;
            }
        }

        public string SessionId { get; private set; }
        #endregion

        #region Constructor
        public WampSession(IWampTransport transport)
        {
            this._pendingCalls = new Dictionary<string, Action<Exception, JToken>>();
            this._messageHandlers = new Dictionary<MessageType, Action<JArray>>();
            this._topics = new Dictionary<string, IWampSubscription>();

            this._messageHandlers[MessageType.WELCOME] = OnWelcome;
            this._messageHandlers[MessageType.CALLRESULT] = OnCallResult;
            this._messageHandlers[MessageType.CALLERROR] = OnCallError;
            this._messageHandlers[MessageType.EVENT] = OnEvent;

            this._transport = transport;
            this._transport.Message += transport_Message;

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

            List<object> arr = new List<object> { MessageType.CALL, call_id, method };

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

            DispatchMessage(arr);

            return source.Task;

        }

        public IWampSubject<T> Subscribe<T>(string topic)
        {
            IWampSubscription found = null;
            IWampSubscription<T> subscription = null;

            if (!this._topics.TryGetValue(topic, out found))
            {
                this._topics[topic] = subscription = new WampSubscription<T>(this, topic);
                DispatchMessage(new object[]{5, topic});
            }
            else
            {
                subscription = (WampSubscription<T>)found;
            }

            return subscription.CreateSubject();
        }

        public void Publish<T>(string topic, T ev)
        {
            DispatchMessage(new object [] {MessageType.SUBSCRIBE, topic, ev});
        }

        public void Unsubscribe(string topic)
        {
            IWampSubscription subscription;
            if (this._topics.TryGetValue(topic, out subscription))
            {
                this._topics.Remove(topic);
                subscription.Dispose();
                DispatchMessage(new object[] { MessageType.UNSUBSCRIBE, topic });
            }
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

            Action<JArray> action;

            if (_messageHandlers.TryGetValue(type, out action))
            {
                action(e.Message);
            }
            else
            {
                //TODO Report errors
            }

             
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

            Action<Exception, JToken> action;

            if (this._pendingCalls.TryGetValue(call_id, out action))
            {
                this._pendingCalls.Remove(call_id);
                action(null, m[2]);
            }
        }

        private void OnEvent(JArray m)
        {
            var topic = m[1].Value<string>();

            IWampSubscription subject = null;

            if (this._topics.TryGetValue(topic, out subject))
            {
                subject.HandleEvent(topic, m[2]);
            }
        }

        private void OnCallError(JArray m)
        {
            var call_id = m[1].Value<string>();

            var exception = new WampCallException("Error on on call to topic: " + m[2].Value<string>());

            Action<Exception, JToken> action;

            if (this._pendingCalls.TryGetValue(call_id, out action))
            {
                this._pendingCalls.Remove(call_id);
                action(exception, default(JToken));
            }
        }

        private void DispatchMessage(IEnumerable<object> array)
        {
            this._transport.Send(array);
        }

        private string GenerateCallId()
        {
            return CryptographicBuffer.EncodeToBase64String(CryptographicBuffer.GenerateRandom(20));
        }


    }
}
