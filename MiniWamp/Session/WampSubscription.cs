using DapperWare.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare
{
    public class WampSubscription<T> : IWampSubscription<T>
    {
        private IWampSession _session;
        private RefCountedDisposable _subscription;
        private CompositeDisposable _subjects;

        public WampSubscription(IWampSession session, string topic)
        {
            this.Topic = topic;
            this._session = session;
            this._subscription = new RefCountedDisposable(Disposable.Create(() => { 
                if (this._session != null)
                    this._session.Unsubscribe(this.Topic);
            }));
            this._subjects = new CompositeDisposable();
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<WampSubscriptionMessageEventArgs<T>> Event;
        

        public void HandleEvent(Newtonsoft.Json.Linq.JToken jToken)
        {
            OnEvent(this, new WampSubscriptionMessageEventArgs<T>(Topic, jToken.ToObject<T>()));
        }

        protected virtual void OnEvent(object sender, WampSubscriptionMessageEventArgs<T> args)
        {
            if (this.Event != null)
            {
                Event(sender, args);
            }
        }

        public string Topic
        {
            get;
            private set;
        }

        public void Dispose()
        {
            this._subjects.Dispose();
            this._subscription.Dispose();
        }

        public IWampSubject<T> CreateSubject()
        {
            var subject = new WampSubject<T>(this, this._subscription.GetDisposable());
            this._subjects.Add(subject);
            return subject;
        }

        IWampSubject IWampSubscription.CreateSubject()
        {
            return CreateSubject();
        }
    }
}


#region Testing
#if DEBUG && !NETFX_CORE

namespace DapperWare
{

    using NUnit.Framework;
    using Moq;
    using Newtonsoft.Json.Linq;

    [TestFixture]
    public class WampSubscriptionTests
    {

        Mock<IWampSession> mockSession;

        [SetUp]
        public void SetUp()
        {
            mockSession = new Mock<IWampSession>();
        }

        [Test]
        public void TestRaiseEvent()
        {
            var subscription = new WampSubscription<int>(mockSession.Object, 
                "http://faketopic.com/something#resource");

            var mockEventHandler = new Mock<EventHandler<WampSubscriptionMessageEventArgs<int>>>();

            subscription.Event += mockEventHandler.Object;

            subscription.HandleEvent(new JValue(5));

            mockEventHandler.Verify(eh => eh(subscription, 
                It.Is<WampSubscriptionMessageEventArgs<int>>(left => left.Value == 5)), Times.Once());
        }

        [Test]
        public void TestUnsubscribeSession()
        {
            var subscription = new WampSubscription<int>(mockSession.Object,
                "http://faketopic.com/something#resource");

            subscription.Dispose();

            mockSession.Verify(sess => sess.Unsubscribe("http://faketopic.com/something#resource"));
        }

        [Test]
        public void TestUnsubscribeSessionWithExistingSubject()
        {
            var subscription = new WampSubscription<int>(mockSession.Object,
                "http://faketopic.com/something#resource");

            var subject = subscription.CreateSubject();

            subscription.Dispose();

            mockSession.Verify(sess => sess.Unsubscribe("http://faketopic.com/something#resource"));

        }

    }

}

#endif

#endregion
