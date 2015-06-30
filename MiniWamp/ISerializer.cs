using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare
{
    /// <summary>
    /// Specialized serializer for serializing an array of objects, the underlying serialization method can be whatever we want
    /// </summary>
    public interface ISerializer
    {
        void Serialize(Windows.Storage.Streams.IDataWriter writer, IEnumerable<object> array);
    }
}
