using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare.Serialization
{
    public class JsonSerializer : ISerializer
    {
        public void Serialize(Windows.Storage.Streams.IDataWriter writer, IEnumerable<object> array)
        {
            var result = Newtonsoft.Json.JsonConvert.SerializeObject(array);
            writer.WriteString(result);
        }
    }
}
