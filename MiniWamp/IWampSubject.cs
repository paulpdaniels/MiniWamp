using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallWamp
{
    public interface IWampSubject
    {
        void Unsubscribe();

        void OnEvent(string topic, Newtonsoft.Json.Linq.JToken ev);

        string Topic { get; }
    }

    public interface IWampSubject<T> : IWampSubject
    {
        event EventHandler<WampSubscriptionMessageEventArgs<T>> Event;

    }
}
