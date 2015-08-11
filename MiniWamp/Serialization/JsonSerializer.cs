using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare.Serialization
{
    public class JsonSerializer : ISerializer
    {
        private Newtonsoft.Json.JsonSerializer _serializer;

        public JsonSerializer()
        {
            this._serializer = new Newtonsoft.Json.JsonSerializer();
        }

        public void Serialize(Stream stream, IEnumerable<object> array)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms))
                {
                    this._serializer.Serialize(writer, array);
                    writer.Flush();

                    ms.Position = 0;
                    ms.CopyTo(stream);
                }
            }
        }
    }
}

#region Testing
#if DEBUG && !NETFX_CORE

namespace DapperWare.Serialization
{
    using NUnit.Framework;

    [TestFixture]
    public class TestJsonSerializer
    {

        private ISerializer serializer;

        [SetUp]
        public void SetUp()
        {
            serializer = new JsonSerializer();
        }


        [Test]
        public void TestSingleValueArray()
        {
            using (var ms = new MemoryStream()) {
                serializer.Serialize(ms, new object[] { 5 });

                var result = Encoding.UTF8.GetString(ms.ToArray());

                Assert.AreEqual(result, "[5]");
            }
        }

        [Test]
        public void TestEmptyArray()
        {
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, new object[] { });
                var result = Encoding.UTF8.GetString(ms.ToArray());
                Assert.AreEqual(result, "[]");
            }
        }

        [Test]
        public void TestVariantTypeArray()
        {
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, new object[] { 5, "blah", true });
                var result = Encoding.UTF8.GetString(ms.ToArray());
                Assert.AreEqual(result, "[5,\"blah\",true]");
            }
        }

    }
}

#endif
#endregion