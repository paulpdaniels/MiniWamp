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

namespace DapperWare
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

            public IWampTransport Create()
            {
                return instance;
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


            public Task ConnectAsync(string url)
            {
                RaiseMessage(new JArray(0, "mysessionid", 1, "test server 1.0"));
                return Task.FromResult(true);
            }

            public void Close() { }


            public event EventHandler Error;


            public void Send(IEnumerable<object> message)
            {
                if (this.SendProxy != null)
                    this.SendProxy(JArray.FromObject(message));
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
            var connectionTask = WampClient.ConnectAsync("ws://localhost:3000", mockTransportFactory);
            connectionTask.Wait();
            this.connection = connectionTask.Result;
        }

        [TestMethod]
        public void TestDeserializeMultiMessage()
        {
            string message = "[3][\"test\"]";

            using (var reader = new JsonTextReader(new StreamReader(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(message)))))
            {
                
                reader.SupportMultipleContent = true;
                var parsed = JArray.Load(reader);
                reader.Read();
                var parsed2 = JArray.Load(reader);

                Assert.AreEqual(1, parsed.Count);
                Assert.AreEqual(1, parsed2.Count);
            }
        }

        [TestMethod]
        public void TestDeserializeMessage()
        {
            string message = "[3]";

            using (var reader = new JsonTextReader(new StreamReader(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(message)))))
            {
                List<JArray> messages = new List<JArray>();
                reader.SupportMultipleContent = true;

                while (reader.Read())
                {
                    messages.Add(JArray.Load(reader));
                }

                Assert.AreEqual(1, messages.Count);
            }
        }
    }
}
