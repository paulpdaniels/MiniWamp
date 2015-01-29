using DapperWare.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare
{
    public class WampSubject<T> : RefCounted, IWampSubject<T>
    {
        private WampSession _session;

        public WampSubject(WampSession session, string topic)
        {
            this.Topic = topic;
            this._session = session;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<WampSubscriptionMessageEventArgs<T>> Event;
        


        public void HandleEvent(string topic, Newtonsoft.Json.Linq.JToken jToken)
        {
            OnEvent(this, new WampSubscriptionMessageEventArgs<T>(topic, jToken.ToObject<T>()));
        }

        internal virtual void OnEvent(object sender, WampSubscriptionMessageEventArgs<T> args)
        {
            if (this.Event != null)
            {
                Event(sender, args);
            }
        }

        public string Topic
        {
            get;
            private set;
        }

        public void Unsubscribe()
        {
                this._session.Unsubscribe(this.Topic);
        }

        protected override void DoDispose()
        {
            Unsubscribe();
        }

        public IWampSubject<T> CreateChild()
        {
            this.Increment();
            return new ChildWampSubject<T>(this, () => this.Release());
        }
    }
}
