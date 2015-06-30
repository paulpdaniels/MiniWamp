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
            this.Subscription = parent;
            this._onUnsubscribe = onUnsubscribe;
            this._handlers = new List<EventHandler<WampSubscriptionMessageEventArgs<T>>>();
        }

        public IWampSubscription<T> Subscription { get; private set; }

        public event EventHandler<WampSubscriptionMessageEventArgs<T>> Event
        {
            add { this.Subscription.Event += value; this._handlers.Add(value); }
            remove { this.Subscription.Event -= value; this._handlers.Remove(value); }
        }

        public void Unsubscribe()
        {
            Detach();
            _onUnsubscribe.Dispose();
        }

        public string Topic
        {
            get { return this.Subscription.Topic; }
        }

        /// <summary>
        /// Stops the ChildSubject from receiving any more updates from the parent
        /// </summary>
        private void Detach()
        {
            foreach (var h in this._handlers)
                Subscription.Event -= h;

            this._handlers.Clear();
            
        }

        public void Dispose()
        {
            this.Unsubscribe();
        }
    }
}
