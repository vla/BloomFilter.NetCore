using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloomFilter.Redis.Test
{
    class Utilitiy
    {
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

       
    }
}
