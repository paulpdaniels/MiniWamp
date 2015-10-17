using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Testing
#if !NETFX_CORE

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
