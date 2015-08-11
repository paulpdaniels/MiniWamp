using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETFX_CORE
using Windows.Foundation.Metadata;
#endif

namespace DapperWare
{
    public interface IWampSubject : IDisposable
    {
        /// <summary>
        /// Unsubscribes from this subject, 
        /// </summary>
#if NETFX_CORE
        [Deprecated("Use .Dispose instead", DeprecationType.Deprecate, 1)]
      
#endif
        [Obsolete("Use .Dispose instead")]
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
