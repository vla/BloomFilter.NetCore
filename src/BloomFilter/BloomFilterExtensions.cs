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
    }
}
