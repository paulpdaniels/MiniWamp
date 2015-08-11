using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare
{
    public interface IWampSubscription : IDisposable
    {

        string Topic { get; }

        /// <summary>
        /// Creates a new WampSubject that is bound to this subscription
        /// </summary>
        /// <returns></returns>
        IWampSubject CreateSubject();

        /// <summary>
        /// Used internally for raising a new event. 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="ev"></param>
        /// <remarks> Users should not rely on this method! </remarks>
        void HandleEvent(string topic, Newtonsoft.Json.Linq.JToken ev);
    }

    public interface IWampSubscription<T> : IWampSubscription
    {
        new IWampSubject<T> CreateSubject();

        event EventHandler<WampSubscriptionMessageEventArgs<T>> Event;
    }
}
