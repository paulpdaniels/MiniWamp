using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare
{
    public class WampSubscriptionMessageEventArgs<T>
    {

        public WampSubscriptionMessageEventArgs(string topic, T t)
        {
            // TODO: Complete member initialization
            this.Topic = topic;
            this.Value = t;
        }

        public string Topic { get; private set; }

        public T Value { get; set; }
    }
}
