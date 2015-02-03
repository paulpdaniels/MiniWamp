using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare.Testing
{
    [TestClass]
    
    public class ParsingTests
    {
        private JsonSerializer serializer;

        [TestInitialize]
        public void SetUp()
        {
            this.serializer = new JsonSerializer();
        }

        [TestMethod]
        [TestCategory("Parsing")]
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
        [TestCategory("Parsing")]
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
