using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare
{
    internal class WampSubject<T> : IWampSubject<T>
    {
        private IDisposable _onUnsubscribe;
        private List<EventHandler<WampSubscriptionMessageEventArgs<T>>> _handlers;

        /// <summary>
        /// Builds a new WampSubject using the parent subscription
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="onUnsubscribe">method for informing the parent of an unsubscribe</param>
        public WampSubject(IWampSubscription<T> parent, IDisposable onUnsubscribe)
        {
            this.Subscription = parent;
            this._onUnsubscribe = onUnsubscribe;
            this._handlers = new List<EventHandler<WampSubscriptionMessageEventArgs<T>>>();
        }

        /// <summary>
        /// Gets the parent subscription
        /// </summary>
        public IWampSubscription<T> Subscription { get; private set; }

        public event EventHandler<WampSubscriptionMessageEventArgs<T>> Event
        {
            add { this.Subscription.Event += value; this._handlers.Add(value); }
            remove { this.Subscription.Event -= value; this._handlers.Remove(value); }
        }

        public void Unsubscribe()
        {
            this.Dispose();
        }


        public string Topic
        {
            get { return this.Subscription.Topic; }
        }

        /// <summary>
        /// Stops this subject from receiving any more updates from the parent
        /// </summary>
        private void Detach()
        {
            foreach (var h in this._handlers)
                Subscription.Event -= h;

            this._handlers.Clear();

            Subscription = null;
            
        }

        public void Dispose()
        {
            Detach();
            _onUnsubscribe.Dispose();
        }
    }
}
