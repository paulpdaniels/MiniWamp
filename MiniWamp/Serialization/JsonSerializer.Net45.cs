using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare.Serialization
{
    class JsonSerializer : ISerializer
    {
        private Newtonsoft.Json.JsonSerializer _serializer;

        public JsonSerializer()
        {
            _serializer = new Newtonsoft.Json.JsonSerializer();
        }

        public void Serialize(System.IO.TextWriter writer, IEnumerable<object> array)
        {
            using (var textWriter = new Newtonsoft.Json.JsonTextWriter(writer) { CloseOutput = false })
            {
                _serializer.Serialize(textWriter, array);
            }
        }
    }
}
