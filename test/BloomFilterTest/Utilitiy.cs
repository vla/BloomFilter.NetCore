using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BloomFilterTest
{
    static class Utilitiy
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        private const string Base62 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GenerateString(int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException($"{nameof(length)} is {length}.");

            var random = new Random(Guid.NewGuid().GetHashCode());
            return new string(
                Enumerable.Repeat(Base62, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
        }

        public static byte[] GenerateBytes(int size = 4)
        {
            if (size < 0) throw new ArgumentOutOfRangeException($"{nameof(size)} is {size}.");

            var buff = new byte[size];
            Rng.GetBytes(buff);
            return buff;
        }

        public static byte[] ToUtf8Bytes(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        public static byte[] FromHex(this string s)
        {
            return Convert.FromHexString(s);
        }

    }
}
