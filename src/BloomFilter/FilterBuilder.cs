using System;

namespace BloomFilter
{
    /// <summary>
    /// BloomFilter Builder
    /// </summary>
    public partial class FilterBuilder
    {
        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IBloomFilter Build(int expectedElements, string name = BloomFilterConstValue.DefaultInMemoryName)
        {
            return Build(expectedElements, 0.01, name);
        }

        /// <summary>
        ///Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IBloomFilter Build(int expectedElements, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultInMemoryName)
        {
            return Build(expectedElements, 0.01, hashMethod, name);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IBloomFilter Build(int expectedElements, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultInMemoryName)
        {
            return Build(expectedElements, 0.01, hashFunction, name);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IBloomFilter Build(int expectedElements, double errorRate, string name = BloomFilterConstValue.DefaultInMemoryName)
        {
            return Build(expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher], name);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IBloomFilter Build(int expectedElements, double errorRate, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultInMemoryName)
        {
            return new FilterMemory(name, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IBloomFilter Build(int expectedElements, double errorRate, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultInMemoryName)
        {
            return new FilterMemory(name, expectedElements, errorRate, hashFunction);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Obsolete("Use non-generic Build")]
        public static Filter<T> Build<T>(int expectedElements, string name = BloomFilterConstValue.DefaultInMemoryName)
        {
            return Build<T>(expectedElements, 0.01, name);
        }

        /// <summary>
        ///Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Obsolete("Use non-generic Build")]
        public static Filter<T> Build<T>(int expectedElements, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultInMemoryName)
        {
            return Build<T>(expectedElements, 0.01, hashMethod, name);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Obsolete("Use non-generic Build")]
        public static Filter<T> Build<T>(int expectedElements, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultInMemoryName)
        {
            return Build<T>(expectedElements, 0.01, hashFunction, name);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Obsolete("Use non-generic Build")]
        public static Filter<T> Build<T>(int expectedElements, double errorRate, string name = BloomFilterConstValue.DefaultInMemoryName)
        {
            return Build<T>(expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher], name);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Obsolete("Use non-generic Build")]
        public static Filter<T> Build<T>(int expectedElements, double errorRate, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultInMemoryName)
        {
            return new FilterMemory<T>(name, expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Obsolete("Use non-generic Build")]
        public static Filter<T> Build<T>(int expectedElements, double errorRate, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultInMemoryName)
        {
            return new FilterMemory<T>(name, expectedElements, errorRate, hashFunction);
        }
    }
}