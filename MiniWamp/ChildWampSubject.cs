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
        public ChildWampSubject(WampSubject<T> parent, Action onUnsubscribe)
        {
            this.Parent = parent;
            this._onUnsubscribe = onUnsubscribe;
        }

        public WampSubject<T> Parent { get; private set; }

        public event EventHandler<WampSubscriptionMessageEventArgs<T>> Event
        {
            add { this.Parent.Event += value; }
            remove { this.Parent.Event -= value; }
        }

        public void Unsubscribe()
        {
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
    }
}
