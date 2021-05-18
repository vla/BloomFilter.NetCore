using BloomFilter.HashAlgorithms;
using System.Collections.Generic;

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
        /// <returns></returns>
        public static IBloomFilter Build(int expectedElements)
        {
            return Build(expectedElements, 0.01);
        }

        /// <summary>
        ///Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static IBloomFilter Build(int expectedElements, HashMethod hashMethod)
        {
            return Build(expectedElements, 0.01, hashMethod);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static IBloomFilter Build(int expectedElements, HashFunction hashFunction)
        {
            return Build(expectedElements, 0.01, hashFunction);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <returns></returns>
        public static IBloomFilter Build(int expectedElements, double errorRate)
        {
            return Build(expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static IBloomFilter Build(int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterMemory(expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static IBloomFilter Build(int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterMemory(expectedElements, errorRate, hashFunction);
        }







        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements)
        {
            return Build<T>(expectedElements, 0.01);
        }

        /// <summary>
        ///Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements, HashMethod hashMethod)
        {
            return Build<T>(expectedElements, 0.01, hashMethod);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements, HashFunction hashFunction)
        {
            return Build<T>(expectedElements, 0.01, hashFunction);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements, double errorRate)
        {
            return Build<T>(expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new FilterMemory<T>(expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new FilterMemory<T>(expectedElements, errorRate, hashFunction);
        }
    }
}