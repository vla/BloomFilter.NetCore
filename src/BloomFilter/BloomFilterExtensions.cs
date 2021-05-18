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

        /// <summary>
        /// Adds the specified elements.
        /// </summary>
        /// <param name="bloomFilter"></param>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<byte[]> elements)
        {
            return elements.Select(e => bloomFilter.Add(e)).ToList();
        }

        /// <summary>
        /// Async Adds the specified elements.
        /// </summary>
        /// <param name="bloomFilter"></param>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public async static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<byte[]> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.AddAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        /// <summary>
        /// Tests whether an elements is present in the filter
        /// </summary>
        /// <param name="bloomFilter"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<byte[]> elements)
        {
            return elements.Select(e => bloomFilter.Contains(e)).ToList();
        }

        /// <summary>
        /// Async Tests whether an elements is present in the filter
        /// </summary>
        /// <param name="bloomFilter"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public async static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<byte[]> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.ContainsAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        /// <summary>
        /// Alls the specified elements.
        /// </summary>
        /// <param name="bloomFilter"></param>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public static bool All(this IBloomFilter bloomFilter, IEnumerable<byte[]> elements)
        {
            return bloomFilter.Contains(elements).All(e => e);
        }

        /// <summary>
        /// Async Alls the specified elements.
        /// </summary>
        /// <param name="bloomFilter"></param>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public async static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<byte[]> elements)
        {
            return (await bloomFilter.ContainsAsync(elements).ConfigureAwait(false)).All(e => e);
        }

        public static bool Add(this IBloomFilter bloomFilter, byte data)
        {
            return bloomFilter.Add(new[] { data });
        }

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, byte data)
        {
            return bloomFilter.AddAsync(new[] { data });
        }

        public static bool Contains(this IBloomFilter bloomFilter, byte data)
        {
            return bloomFilter.Contains(new[] { data });
        }

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, byte data)
        {
            return bloomFilter.ContainsAsync(new[] { data });
        }

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<byte> elements)
        {
            return elements.Select(e => bloomFilter.Add(e)).ToList();
        }

        public async static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<byte> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.AddAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<byte> elements)
        {
            return elements.Select(e => bloomFilter.Contains(e)).ToList();
        }

        public async static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<byte> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.ContainsAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<byte> elements)
        {
            return bloomFilter.Contains(elements).All(e => e);
        }

        public async static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<byte> elements)
        {
            return (await bloomFilter.ContainsAsync(elements).ConfigureAwait(false)).All(e => e);
        }

        #endregion Byte

        #region String

        public static bool Add(this IBloomFilter bloomFilter, string data)
        {
            return bloomFilter.Add(Encoding.UTF8.GetBytes(data));
        }

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, string data)
        {
            return bloomFilter.AddAsync(Encoding.UTF8.GetBytes(data));
        }

        public static bool Contains(this IBloomFilter bloomFilter, string data)
        {
            return bloomFilter.Contains(Encoding.UTF8.GetBytes(data));
        }

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, string data)
        {
            return bloomFilter.ContainsAsync(Encoding.UTF8.GetBytes(data));
        }

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<string> elements)
        {
            return elements.Select(e => bloomFilter.Add(e)).ToList();
        }

        public async static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<string> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.AddAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<string> elements)
        {
            return elements.Select(e => bloomFilter.Contains(e)).ToList();
        }

        public async static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<string> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.ContainsAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<string> elements)
        {
            return bloomFilter.Contains(elements).All(e => e);
        }

        public async static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<string> elements)
        {
            return (await bloomFilter.ContainsAsync(elements).ConfigureAwait(false)).All(e => e);
        }

        #endregion String

        #region Double

        public static bool Add(this IBloomFilter bloomFilter, double data)
        {
            return bloomFilter.Add(BitConverter.GetBytes(data));
        }

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, double data)
        {
            return bloomFilter.AddAsync(BitConverter.GetBytes(data));
        }

        public static bool Contains(this IBloomFilter bloomFilter, double data)
        {
            return bloomFilter.Contains(BitConverter.GetBytes(data));
        }

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, double data)
        {
            return bloomFilter.ContainsAsync(BitConverter.GetBytes(data));
        }

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<double> elements)
        {
            return elements.Select(e => bloomFilter.Add(e)).ToList();
        }

        public async static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<double> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.AddAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<double> elements)
        {
            return elements.Select(e => bloomFilter.Contains(e)).ToList();
        }

        public async static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<double> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.ContainsAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<double> elements)
        {
            return bloomFilter.Contains(elements).All(e => e);
        }

        public async static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<double> elements)
        {
            return (await bloomFilter.ContainsAsync(elements).ConfigureAwait(false)).All(e => e);
        }

        #endregion Double

        #region Single

        public static bool Add(this IBloomFilter bloomFilter, float data)
        {
            return bloomFilter.Add(BitConverter.GetBytes(data));
        }

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, float data)
        {
            return bloomFilter.AddAsync(BitConverter.GetBytes(data));
        }

        public static bool Contains(this IBloomFilter bloomFilter, float data)
        {
            return bloomFilter.Contains(BitConverter.GetBytes(data));
        }

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, float data)
        {
            return bloomFilter.ContainsAsync(BitConverter.GetBytes(data));
        }

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<float> elements)
        {
            return elements.Select(e => bloomFilter.Add(e)).ToList();
        }

        public async static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<float> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.AddAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<float> elements)
        {
            return elements.Select(e => bloomFilter.Contains(e)).ToList();
        }

        public async static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<float> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.ContainsAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<float> elements)
        {
            return bloomFilter.Contains(elements).All(e => e);
        }

        public async static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<float> elements)
        {
            return (await bloomFilter.ContainsAsync(elements).ConfigureAwait(false)).All(e => e);
        }

        #endregion Single

        #region Int16

        public static bool Add(this IBloomFilter bloomFilter, short data)
        {
            return bloomFilter.Add(BitConverter.GetBytes(data));
        }

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, short data)
        {
            return bloomFilter.AddAsync(BitConverter.GetBytes(data));
        }

        public static bool Contains(this IBloomFilter bloomFilter, short data)
        {
            return bloomFilter.Contains(BitConverter.GetBytes(data));
        }

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, short data)
        {
            return bloomFilter.ContainsAsync(BitConverter.GetBytes(data));
        }

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<short> elements)
        {
            return elements.Select(e => bloomFilter.Add(e)).ToList();
        }

        public async static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<short> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.AddAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<short> elements)
        {
            return elements.Select(e => bloomFilter.Contains(e)).ToList();
        }

        public async static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<short> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.ContainsAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<short> elements)
        {
            return bloomFilter.Contains(elements).All(e => e);
        }

        public async static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<short> elements)
        {
            return (await bloomFilter.ContainsAsync(elements).ConfigureAwait(false)).All(e => e);
        }

        #endregion Int16

        #region Int32

        public static bool Add(this IBloomFilter bloomFilter, int data)
        {
            return bloomFilter.Add(BitConverter.GetBytes(data));
        }

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, int data)
        {
            return bloomFilter.AddAsync(BitConverter.GetBytes(data));
        }

        public static bool Contains(this IBloomFilter bloomFilter, int data)
        {
            return bloomFilter.Contains(BitConverter.GetBytes(data));
        }

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, int data)
        {
            return bloomFilter.ContainsAsync(BitConverter.GetBytes(data));
        }

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<int> elements)
        {
            return elements.Select(e => bloomFilter.Add(e)).ToList();
        }

        public async static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<int> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.AddAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<int> elements)
        {
            return elements.Select(e => bloomFilter.Contains(e)).ToList();
        }

        public async static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<int> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.ContainsAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<int> elements)
        {
            return bloomFilter.Contains(elements).All(e => e);
        }

        public async static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<int> elements)
        {
            return (await bloomFilter.ContainsAsync(elements).ConfigureAwait(false)).All(e => e);
        }

        #endregion Int32

        #region Int64

        public static bool Add(this IBloomFilter bloomFilter, long data)
        {
            return bloomFilter.Add(BitConverter.GetBytes(data));
        }

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, long data)
        {
            return bloomFilter.AddAsync(BitConverter.GetBytes(data));
        }

        public static bool Contains(this IBloomFilter bloomFilter, long data)
        {
            return bloomFilter.Contains(BitConverter.GetBytes(data));
        }

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, long data)
        {
            return bloomFilter.ContainsAsync(BitConverter.GetBytes(data));
        }

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<long> elements)
        {
            return elements.Select(e => bloomFilter.Add(e)).ToList();
        }

        public async static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<long> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.AddAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<long> elements)
        {
            return elements.Select(e => bloomFilter.Contains(e)).ToList();
        }

        public async static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<long> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.ContainsAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<long> elements)
        {
            return bloomFilter.Contains(elements).All(e => e);
        }

        public async static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<long> elements)
        {
            return (await bloomFilter.ContainsAsync(elements).ConfigureAwait(false)).All(e => e);
        }

        #endregion Int64

        #region UInt16

        public static bool Add(this IBloomFilter bloomFilter, ushort data)
        {
            return bloomFilter.Add(BitConverter.GetBytes(data));
        }

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, ushort data)
        {
            return bloomFilter.AddAsync(BitConverter.GetBytes(data));
        }

        public static bool Contains(this IBloomFilter bloomFilter, ushort data)
        {
            return bloomFilter.Contains(BitConverter.GetBytes(data));
        }

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, ushort data)
        {
            return bloomFilter.ContainsAsync(BitConverter.GetBytes(data));
        }

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<ushort> elements)
        {
            return elements.Select(e => bloomFilter.Add(e)).ToList();
        }

        public async static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<ushort> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.AddAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<ushort> elements)
        {
            return elements.Select(e => bloomFilter.Contains(e)).ToList();
        }

        public async static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<ushort> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.ContainsAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<ushort> elements)
        {
            return bloomFilter.Contains(elements).All(e => e);
        }

        public async static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<ushort> elements)
        {
            return (await bloomFilter.ContainsAsync(elements).ConfigureAwait(false)).All(e => e);
        }

        #endregion UInt16

        #region UInt32

        public static bool Add(this IBloomFilter bloomFilter, uint data)
        {
            return bloomFilter.Add(BitConverter.GetBytes(data));
        }

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, uint data)
        {
            return bloomFilter.AddAsync(BitConverter.GetBytes(data));
        }

        public static bool Contains(this IBloomFilter bloomFilter, uint data)
        {
            return bloomFilter.Contains(BitConverter.GetBytes(data));
        }

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, uint data)
        {
            return bloomFilter.ContainsAsync(BitConverter.GetBytes(data));
        }

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<uint> elements)
        {
            return elements.Select(e => bloomFilter.Add(e)).ToList();
        }

        public async static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<uint> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.AddAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<uint> elements)
        {
            return elements.Select(e => bloomFilter.Contains(e)).ToList();
        }

        public async static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<uint> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.ContainsAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<uint> elements)
        {
            return bloomFilter.Contains(elements).All(e => e);
        }

        public async static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<uint> elements)
        {
            return (await bloomFilter.ContainsAsync(elements).ConfigureAwait(false)).All(e => e);
        }

        #endregion UInt32

        #region UInt64

        public static bool Add(this IBloomFilter bloomFilter, ulong data)
        {
            return bloomFilter.Add(BitConverter.GetBytes(data));
        }

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, ulong data)
        {
            return bloomFilter.AddAsync(BitConverter.GetBytes(data));
        }

        public static bool Contains(this IBloomFilter bloomFilter, ulong data)
        {
            return bloomFilter.Contains(BitConverter.GetBytes(data));
        }

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, ulong data)
        {
            return bloomFilter.ContainsAsync(BitConverter.GetBytes(data));
        }

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<ulong> elements)
        {
            return elements.Select(e => bloomFilter.Add(e)).ToList();
        }

        public async static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<ulong> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.AddAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<ulong> elements)
        {
            return elements.Select(e => bloomFilter.Contains(e)).ToList();
        }

        public async static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<ulong> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.ContainsAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<ulong> elements)
        {
            return bloomFilter.Contains(elements).All(e => e);
        }

        public async static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<ulong> elements)
        {
            return (await bloomFilter.ContainsAsync(elements).ConfigureAwait(false)).All(e => e);
        }

        #endregion UInt64

        #region DateTime

        public static bool Add(this IBloomFilter bloomFilter, DateTime data)
        {
            return bloomFilter.Add(BitConverter.GetBytes(data.Ticks));
        }

        public static Task<bool> AddAsync(this IBloomFilter bloomFilter, DateTime data)
        {
            return bloomFilter.AddAsync(BitConverter.GetBytes(data.Ticks));
        }

        public static bool Contains(this IBloomFilter bloomFilter, DateTime data)
        {
            return bloomFilter.Contains(BitConverter.GetBytes(data.Ticks));
        }

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, DateTime data)
        {
            return bloomFilter.ContainsAsync(BitConverter.GetBytes(data.Ticks));
        }

        public static IList<bool> Add(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements)
        {
            return elements.Select(e => bloomFilter.Add(e)).ToList();
        }

        public async static Task<IList<bool>> AddAsync(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.AddAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static IList<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements)
        {
            return elements.Select(e => bloomFilter.Contains(e)).ToList();
        }

        public async static Task<IList<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await bloomFilter.ContainsAsync(el).ConfigureAwait(false));
            }
            return result;
        }

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements)
        {
            return bloomFilter.Contains(elements).All(e => e);
        }

        public async static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements)
        {
            return (await bloomFilter.ContainsAsync(elements).ConfigureAwait(false)).All(e => e);
        }

        #endregion DateTime
    }
}