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
