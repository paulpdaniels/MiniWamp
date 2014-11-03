using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallWamp
{
    public class WampSubscriptionMessageEventArgs<T>
    {
        private string topic;
        private T t;

        public WampSubscriptionMessageEventArgs(string topic, T t)
        {
            // TODO: Complete member initialization
            this.topic = topic;
            this.t = t;
        }
    }
}
