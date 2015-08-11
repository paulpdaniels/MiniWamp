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
        void Serialize(System.IO.Stream writer, IEnumerable<object> array);
    }
}
