using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare
{
    class WampSubject<T> : IWampSubject<T>
    {
        private IDisposable _onUnsubscribe;
        private List<EventHandler<WampSubscriptionMessageEventArgs<T>>> _handlers;

        public WampSubject(WampSubscription<T> parent, IDisposable onUnsubscribe)
        {
            this.Parent = parent;
            this._onUnsubscribe = onUnsubscribe;
            this._handlers = new List<EventHandler<WampSubscriptionMessageEventArgs<T>>>();
        }

        public WampSubscription<T> Parent { get; private set; }

        public event EventHandler<WampSubscriptionMessageEventArgs<T>> Event
        {
            add { this.Parent.Event += value; this._handlers.Add(value); }
            remove { this.Parent.Event -= value; this._handlers.Remove(value); }
        }

        public void Unsubscribe()
        {
            Detach();
            _onUnsubscribe.Dispose();
        }

        public void HandleEvent(string topic, Newtonsoft.Json.Linq.JToken ev)
        {
            this.Parent.HandleEvent(topic, ev);
        }

        public string Topic
        {
            get { return this.Parent.Topic; }
        }

        /// <summary>
        /// Stops the ChildSubject from receiving any more updates from the parent
        /// </summary>
        private void Detach()
        {
            foreach (var h in this._handlers)
                Parent.Event -= h;

            this._handlers.Clear();
            
        }

        public void Dispose()
        {
            this.Unsubscribe();
        }
    }
}
