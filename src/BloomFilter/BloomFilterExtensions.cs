using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloomFilter
{
    /// <summary>
    /// BloomFilterExtensions
    /// </summary>
    public static class BloomFilterExtensions
    {
        #region Byte

        public static bool Add(this IBloomFilter bloomFilter, byte data) => bloomFilter.Add(new[] { data });

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, byte data) => bloomFilter.AddAsync(new[] { data });

        public static bool Contains(this IBloomFilter bloomFilter, byte data) => bloomFilter.Contains(new[] { data });

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, byte data) => bloomFilter.ContainsAsync(new[] { data });

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.Add(elements.Select(data => new byte[] { data }));

        public static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.AddAsync(elements.Select(data => new byte[] { data }));

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.Contains(elements.Select(data => new byte[] { data }));

        public static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.ContainsAsync(elements.Select(data => new byte[] { data }));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.All(elements.Select(data => new byte[] { data }));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.AllAsync(elements.Select(data => new byte[] { data }));

        #endregion Byte

        #region String

        public static bool Add(this IBloomFilter bloomFilter, string data) => bloomFilter.Add(Encoding.UTF8.GetBytes(data));

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, string data) => bloomFilter.AddAsync(Encoding.UTF8.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, string data) => bloomFilter.Contains(Encoding.UTF8.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, string data) => bloomFilter.ContainsAsync(Encoding.UTF8.GetBytes(data));

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.Add(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.AddAsync(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.Contains(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.All(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.AllAsync(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        #endregion String

        #region Double

        public static bool Add(this IBloomFilter bloomFilter, double data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, double data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, double data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, double data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        #endregion Double

        #region Single

        public static bool Add(this IBloomFilter bloomFilter, float data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, float data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, float data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, float data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        #endregion Single

        #region Int16

        public static bool Add(this IBloomFilter bloomFilter, short data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, short data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, short data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, short data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        #endregion Int16

        #region Int32

        public static bool Add(this IBloomFilter bloomFilter, int data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, int data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, int data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, int data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        #endregion Int32

        #region Int64

        public static bool Add(this IBloomFilter bloomFilter, long data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, long data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, long data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, long data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        #endregion Int64

        #region UInt16

        public static bool Add(this IBloomFilter bloomFilter, ushort data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, ushort data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, ushort data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, ushort data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        #endregion UInt16

        #region UInt32

        public static bool Add(this IBloomFilter bloomFilter, uint data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, uint data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, uint data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, uint data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        #endregion UInt32

        #region UInt64

        public static bool Add(this IBloomFilter bloomFilter, ulong data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, ulong data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, ulong data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, ulong data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        #endregion UInt64

        #region DateTime

        public static bool Add(this IBloomFilter bloomFilter, DateTime data) => bloomFilter.Add(BitConverter.GetBytes(data.Ticks));

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, DateTime data) => bloomFilter.AddAsync(BitConverter.GetBytes(data.Ticks));

        public static bool Contains(this IBloomFilter bloomFilter, DateTime data) => bloomFilter.Contains(BitConverter.GetBytes(data.Ticks));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, DateTime data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data.Ticks));

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        #endregion DateTime
    }
}