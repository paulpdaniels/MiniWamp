using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SmallWamp;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SmallWamp
{
    [TestClass]
    public class WampSessionTests
    {
        public class MockTransportFactory : ITransportFactory
        {
            private IWampTransport instance;

            public MockTransportFactory(IWampTransport instance)
            {
                this.instance = instance;
            }

            public Task<IWampTransport> CreateAsync(string url)
            {
                return Task.FromResult(instance);
            }
        }

        public class MockWampTransport : IWampTransport
        {

            public event EventHandler Closed;

            public void Send(Newtonsoft.Json.Linq.JToken array)
            {
                if (this.SendProxy != null)
                    this.SendProxy(array);
            }

            public Action<JToken> SendProxy { get; set; }

            public event EventHandler<WampMessageEventArgs> Message;

            internal void RaiseMessage(JArray result)
            {
                if (Message != null)
                {
                    this.Message(this, new WampMessageEventArgs(result));
                }
            }

        }

        private ITransportFactory mockTransportFactory;
        private MockWampTransport mockTransport;
        private WampSession connection;


        [TestInitialize]
        public void Setup()
        {
            mockTransport = new MockWampTransport();
            mockTransportFactory = new MockTransportFactory(mockTransport);
            var connectionTask = WampClient.Connect("ws://localhost:3000", mockTransportFactory);
            connectionTask.Wait();
            this.connection = connectionTask.Result;
        }

        [TestMethod]
        public void TestPublishEvent()
        {
            connection.Publish("test/topic", 5);
        }

        [TestMethod]
        public void TestSubscribe()
        {
            var t = connection.Subscribe<int>("test/topic");
            Assert.AreEqual(t.Topic, "test/topic");
            Assert.AreEqual(connection.Subscriptions.Count(), 1);
        }

        [TestMethod]
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
        public void TestCallMethod()
        {
            mockTransport.SendProxy = arr =>
            {
                JArray call = (JArray)arr;
                //Must capture the call id in order finish the call
                JArray result = new JArray(MessageType.CALLRESULT, call[2], 8);
                mockTransport.RaiseMessage(result);
            };


            var callTask = connection.Call<int>("test/method", 5, 3);

            callTask.Wait();

            Assert.AreEqual(callTask.Result, 8);
        }

        [TestMethod]
        public void TestCallWithError()
        {
            mockTransport.SendProxy = arr =>
            {
                JArray call = (JArray)arr;
                JArray result = new JArray(MessageType.CALLERROR, call[2], 8);
                mockTransport.RaiseMessage(result);
            };

            var callTask = connection.Call<int>("test/method", 5, 3);

            Assert.ThrowsException<AggregateException>(() => callTask.Wait());

        }
    }
}
