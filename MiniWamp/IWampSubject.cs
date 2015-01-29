using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare
{
    public interface IWampSubject
    {
        /// <summary>
        /// Unsubscribes from this subject, 
        /// </summary>
        void Unsubscribe();

        /// <summary>
        /// Used internally for raising a new event. 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="ev"></param>
        /// <remarks> Users should not rely on this method! </remarks>
        void HandleEvent(string topic, Newtonsoft.Json.Linq.JToken ev);

        /// <summary>
        /// Gets the topic for this subject
        /// </summary>
        string Topic { get; }
    }

    public interface IWampSubject<T> : IWampSubject
    {
        event EventHandler<WampSubscriptionMessageEventArgs<T>> Event;

    }
}
