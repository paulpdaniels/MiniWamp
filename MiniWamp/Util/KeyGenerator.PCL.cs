using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;

namespace DapperWare.Util
{
    public class KeyGenerator
    {

        public static string GenerateKey(int maxSize)
        {
            return CryptographicBuffer.EncodeToBase64String(CryptographicBuffer.GenerateRandom((uint)maxSize));
        }

    }
}
