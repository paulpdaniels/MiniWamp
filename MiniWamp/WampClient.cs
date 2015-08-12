using DapperWare.Session;
using DapperWare.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare
{
    public class WampClient
    {
        /// <summary>
        /// Identical to calling <code>Connect(url, MessageWebSocketTransportFactory.Default)</code>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<IWampSession> ConnectAsync(string url)
        {
            return ConnectAsync(url, DefaultTransportFactory.Default);
        }

        /// <summary>
        /// Creates a new WampSession by invoking the create method on the provided factory then asynchronously connects
        /// Equivalent to:<br>
        /// <code>
        ///     var session = new WampSession(factory.Create());
        ///     await session.ConnectAsync(url);
        /// </code>
        /// </br>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static Task<IWampSession> ConnectAsync(string url, ITransportFactory factory)
        {
            return ConnectAsync(url, () => factory.Create());
        }

        /// <summary>
        /// Creates a new WampSession by invoking the provided factory function then asynchronously connects
        /// </summary>
        /// <param name="url"></param>
        /// <param name="factoryFn"></param>
        /// <returns></returns>
        public static async Task<IWampSession> ConnectAsync(string url, Func<IWampTransport> factoryFn)
        {
            var connection = new WampConnection(url, factoryFn());

            //var session = new WampSession(factoryFn());

            //await session.ConnectAsync(url);

            return await connection.Open();
        }

    }
}


#region Testing

#if DEBUG && !NETFX_CORE

namespace DapperWare {

    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class TestWampClient {

        private Mock<IWampTransport> mockTransport;

        private IWampSession connection;

        [SetUp]
        public async void SetUp()
        {
            mockTransport = new Mock<IWampTransport>();

            mockTransport
                .Setup(t => t.ConnectAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(false))
                .Raises(t => t.Message += null, new WampMessageEventArgs(
                    new JArray(0, "mysessionid", 1, "moqs")));

            var mock = new Mock<IWampSession>();

            connection = await WampClient.ConnectAsync("ws://localhost:3000", () => mockTransport.Object);
        }

        [Test]
        public void TestInitializedSession()
        {
            Assert.AreEqual("mysessionid", connection.SessionId);
        }

        [Test]
        public void TestPublishEvent()
        {
            List<object> result = null;

            mockTransport.Setup(t => t.Send(It.IsAny<IEnumerable<object>>()))
                .Callback<IEnumerable<object>>(e => result = e.ToList());

            connection.Publish("test/topic", 5);

            CollectionAssert.AreEqual(result, new object[] {MessageType.SUBSCRIBE, "test/topic", 5});

        }

        [Test]
        public void TestSubscribe()
        {
            var t = connection.Subscribe<int>("test/topic");
            Assert.AreEqual(t.Topic, "test/topic");
            Assert.AreEqual(connection.Subscriptions.Count(), 1);
        }

        [Test]
        public void TestUnsubscribe()
        {
            connection.Subscribe<int>("test/topic");
            using (var t = connection.Subscribe<int>("test/topic2"))
            {
                Assert.AreEqual(connection.Subscriptions.Count(), 2);
                connection.Unsubscribe("test/topic");
                Assert.AreEqual(connection.Subscriptions.Count(), 1);
            }

            Assert.AreEqual(connection.Subscriptions.Count(), 0);
        }

        [Test]
        public async void TestCallMethod()
        {
            List<object> list = null;
            var setup = mockTransport.Setup(t => t.Send(It.IsAny<IEnumerable<object>>()));

            setup
                .Callback<IEnumerable<object>>(e => list = e.ToList());

            setup
                .Raises(t => t.Message += null, () => new WampMessageEventArgs(new JArray(
                    3, list[1], 8
                    )));

            var result = await connection.Call<int>("test/method", 5, 3);

            Assert.AreEqual(result, 8);
        }

        [Test]
        public async void TestCallWithError()
        {
            List<object> list = null;
            var setup = mockTransport.Setup(t => t.Send(It.IsAny<IEnumerable<object>>()));

            setup
                .Callback<IEnumerable<object>>(e => list = e.ToList());

            setup
                .Raises(t => t.Message += null, () => new WampMessageEventArgs(new JArray(
                    4, list[1], "http://badexample.com", "")));

            try
            {
                var result = await connection.Call<int>("test/method", 5, 3);
            }
            catch (WampCallException e)
            {
                Assert.Pass();
            }

            Assert.Fail("Should have thrown an exception");
        }


    }

}


#endif
#endregion