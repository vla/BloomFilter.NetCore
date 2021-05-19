using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloomFilter
{
    /// <summary>
    /// Represents a Bloom filter.
    /// </summary>
    public interface IBloomFilter : IDisposable
    {
        /// <summary>
        /// Gets the name specified by BloomFilter.
        /// </summary>
        string Name { get; }

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
        /// Adds the specified elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        IList<bool> Add(IEnumerable<byte[]> elements);

        /// <summary>
        /// Async Adds the specified elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        Task<IList<bool>> AddAsync(IEnumerable<byte[]> elements);

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
        /// Tests whether an elements is present in the filter
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        IList<bool> Contains(IEnumerable<byte[]> elements);

        /// <summary>
        /// Async Tests whether an elements is present in the filter
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        Task<IList<bool>> ContainsAsync(IEnumerable<byte[]> elements);

        /// <summary>
        /// Alls the specified elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        bool All(IEnumerable<byte[]> elements);

        /// <summary>
        /// Async Alls the specified elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        Task<bool> AllAsync(IEnumerable<byte[]> elements);

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