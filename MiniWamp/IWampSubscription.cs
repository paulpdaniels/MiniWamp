using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare
{
    public interface IWampSubscription : IDisposable
    {
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
        IWampSubject<T> CreateSubject();
    }
}
