using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation.Metadata;

namespace DapperWare
{
    public interface IWampSubject : IDisposable
    {
        /// <summary>
        /// Unsubscribes from this subject, 
        /// </summary>
        [Deprecated("Use .Dispose instead", DeprecationType.Deprecate, 1)]
        void Unsubscribe();

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
