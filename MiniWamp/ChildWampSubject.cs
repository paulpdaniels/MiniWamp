using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare
{
    class ChildWampSubject<T> : IWampSubject<T>
    {
        private Action _onUnsubscribe;
        private List<EventHandler<WampSubscriptionMessageEventArgs<T>>> _handlers;
        


        public ChildWampSubject(WampSubject<T> parent, Action onUnsubscribe)
        {
            this.Parent = parent;
            this._onUnsubscribe = onUnsubscribe;
            this._handlers = new List<EventHandler<WampSubscriptionMessageEventArgs<T>>>();
            //Attach();
        }

        public WampSubject<T> Parent { get; private set; }

        public event EventHandler<WampSubscriptionMessageEventArgs<T>> Event
        {
            add { this.Parent.Event += value; this._handlers.Add(value); }
            remove { this.Parent.Event -= value; this._handlers.Remove(value); }
        }

        public void Unsubscribe()
        {
            Detach();
            _onUnsubscribe();
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

    }
}
