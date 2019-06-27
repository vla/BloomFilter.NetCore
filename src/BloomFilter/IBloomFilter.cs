using System;
using System.Threading.Tasks;

namespace BloomFilter
{
    /// <summary>
    /// Represents a Bloom filter.
    /// </summary>
    public interface IBloomFilter: IDisposable
    {
        /// <summary>
        /// Adds the passed value to the filter.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        bool Add(byte[] element);

        /// <summary>
        /// Async Adds the passed value to the filter.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        Task<bool> AddAsync(byte[] element);

        /// <summary>
        /// Tests whether an element is present in the filter
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        bool Contains(byte[] element);

        /// <summary>
        /// Async Tests whether an element is present in the filter
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        Task<bool> ContainsAsync(byte[] element);

        /// <summary>
        /// Removes all elements from the filter
        /// </summary>
        void Clear();

        /// <summary>
        /// Async Removes all elements from the filter
        /// </summary>
        Task ClearAsync();

        /// <summary>
        ///  Hashes the specified value.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int[] ComputeHash(byte[] data);
    }
}