using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using DapperWare;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace DapperWare.Testing
{
    [TestClass]
    public class WampSessionTests
    {
        private MockWampTransport mockTransport;
        private WampSession connection;


        [TestInitialize]
        public void Setup()
        {
            mockTransport = new MockWampTransport();
            var connectionTask = WampClient.ConnectAsync("ws://localhost:3000", () => mockTransport);
            connectionTask.Wait();
            this.connection = connectionTask.Result;
        }

        [TestCleanup]
        public void TearDown()
        {
            mockTransport.Reset();
        }

        [TestMethod]
        [TestCategory("Initialization")]
        public void TestInitializedSession()
        {
            Assert.AreEqual(connection.SessionId, "mysessionid");
        }

        [TestMethod]
        [TestCategory("PubSub")]
        public void TestPublishEvent()
        {
            connection.Publish("test/topic", 5);
        }

        [TestMethod]
        [TestCategory("PubSub")]
        public void TestSubscribe()
        {
            var t = connection.Subscribe<int>("test/topic");
            Assert.AreEqual(t.Topic, "test/topic");
            Assert.AreEqual(1, connection.Subscriptions.Count());
        }

        [TestMethod]
        [TestCategory("PubSub")]
        public void TestUnsubscribe()
        {
            connection.Subscribe<int>("test/topic");
            var t = connection.Subscribe<int>("test/topic2");

            Assert.AreEqual(connection.Subscriptions.Count(), 2);

            connection.Unsubscribe("test/topic");

            Assert.AreEqual(connection.Subscriptions.Count(), 1);

            t.Unsubscribe();

            Assert.AreEqual(connection.Subscriptions.Count(), 0);
        }

        [TestMethod]
        [TestCategory("PubSub")]
        public void TestRefCountedUnsubscribe()
        {
            var sub1 = connection.Subscribe<int>("test/topic");
            var sub2 = connection.Subscribe<int>("test/topic");

            Assert.AreEqual(1, connection.Subscriptions.Count());

            sub1.Unsubscribe();

            Assert.AreEqual(1, connection.Subscriptions.Count());

            sub2.Unsubscribe();

            Assert.AreEqual(0, connection.Subscriptions.Count());
        }

        [TestMethod]
        [TestCategory("PubSub")]
        public void TestUnsubscribeBypassRefCount()
        {
            var sub1 = connection.Subscribe<int>("test/topic");
            var sub2 = connection.Subscribe<int>("test/topic");

            connection.Unsubscribe("test/topic");

            Assert.AreEqual(0, connection.Subscriptions.Count());
        }

        [TestMethod]
        [TestCategory("PubSub")]
        public void TestUnsubscribeCancelListeners()
        {
            var sub1 = connection.Subscribe<int>("test/topic");

            sub1.Event += (sender, e) => { Assert.Fail(); };

            sub1.Unsubscribe();

            sub1.HandleEvent("test/topic", new JValue(42));


        }

        [TestMethod]
        [TestCategory("PubSub")]
        public void TestResubscribe()
        {
            var sub1 = connection.Subscribe<int>("test/topic");

            sub1.Unsubscribe();

            Assert.AreEqual(2, mockTransport.Messages.Count());

            connection.Subscribe<int>("test/topic");

            Assert.AreEqual(3, mockTransport.Messages.Count());
        }

        [TestMethod]
        [TestCategory("PubSub")]
        public void TestUnsubscribeSendsMessage()
        {
            var sub1 = connection.Subscribe<int>("test/topic");

            sub1.Unsubscribe();

            Assert.IsTrue(
                JArray.EqualityComparer.Equals(
                new JArray(MessageType.UNSUBSCRIBE, "test/topic"),
                mockTransport.Messages.ElementAt(1)));
        }

        [TestMethod]
        [TestCategory("RPC")]
        public void TestCallMethod()
        {
            mockTransport.SendProxy = arr =>
            {
                JArray call = (JArray)arr;
                //Must capture the call id in order finish the call
                JArray result = new JArray(MessageType.CALLRESULT, call[1], 8);
                mockTransport.RaiseMessage(result);
            };


            var callTask = connection.Call<int>("test/method", 5, 3);

            callTask.Wait();

            Assert.AreEqual(callTask.Result, 8);
        }

        [TestMethod]
        [TestCategory("RPC")]
        public void TestCallWithError()
        {
            mockTransport.SendProxy = arr =>
            {
                JArray call = (JArray)arr;
                JArray result = new JArray(MessageType.CALLERROR, call[1], 8);
                mockTransport.RaiseMessage(result);
            };

            var callTask = connection.Call<int>("test/method", 5, 3);

            Assert.ThrowsException<AggregateException>(() => callTask.Wait());

        }

    }
}
