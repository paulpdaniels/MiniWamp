using System;
using System.Collections.Generic;


namespace DapperWare
{
    public interface IWampSession
    {
        /// <summary>
        /// Gets the Map of the prefixes that are being used by this session
        /// </summary>
        DapperWare.Util.Map<string, string> Prefixes { get; }

        /// <summary>
        /// Gets the session id for this session
        /// </summary>
        /// <remarks>
        /// This can be null if the session has not been initialized yet
        /// </remarks>
        string SessionId { get; }

        /// <summary>
        /// Gets the set of subscriptions currently held by this session
        /// </summary>
        IEnumerable<IWampSubscription> Subscriptions { get; }

        /// <summary>
        /// Gets the transport used for this session
        /// </summary>
        DapperWare.IWampTransport Transport { get; }

        /// <summary>
        /// Invokes a call on the server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<T> Call<T>(string method, params object[] content);

        /// <summary>
        /// Shuts down this session, unsubscribes all topics
        /// </summary>
        void Close();

        /// <summary>
        /// Publishes a message for other clients
        /// </summary>
        /// <typeparam name="T">The type of the object being sent as the payload</typeparam>
        /// <param name="topic"></param>
        /// <param name="payload"></param>
        /// <param name="excludeMe"></param>
        void Publish<T>(string topic, T payload, bool excludeMe = false);


        void Publish<T>(string topic, T payload, IEnumerable<string> exclude, IEnumerable<string> eligible);

        /// <summary>
        /// Creates a new subscription for a topic and returns a subject for listening to it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topic"></param>
        /// <returns></returns>
        IWampSubject<T> Subscribe<T>(string topic);

        /// <summary>
        /// Unsubscribes from all topics
        /// </summary>
        void Unsubscribe();

        /// <summary>
        /// Unsubscribes completely from a given topic
        /// </summary>
        /// <remarks>
        /// This will dispose of any IWampSubjects for this topic
        /// </remarks>
        /// <param name="topic"></param>
        void Unsubscribe(string topic);
    }
}
