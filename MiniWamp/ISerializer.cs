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
        void Serialize(
#if NETFX_CORE
            Windows.Storage.Streams.IDataWriter writer, 
#else
            System.IO.TextWriter writer,
#endif
            
            IEnumerable<object> array);
    }
}
