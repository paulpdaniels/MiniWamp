using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare
{
    [TestClass]
    class ParsingTests
    {
        private JsonSerializer serializer;

        [TestInitialize]
        public void SetUp()
        {
            this.serializer = new JsonSerializer();
        }

        [TestMethod]
        public void TestDeserializeMessage()
        {
            string message = "[]";

            using (var reader  = new JsonTextReader(new StreamReader(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(message)))))
            {
                var parsed = JArray.Load(reader);

                Assert.AreEqual(0, parsed.Count);
            }


        }

    }
}
