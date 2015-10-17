using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Testing
#if !NETFX_CORE

namespace DapperWare.Serialization
{
    using NUnit.Framework;
    using System.IO;

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
            using (var ms = new MemoryStream())
            {
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
