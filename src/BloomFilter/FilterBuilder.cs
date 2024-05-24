using BloomFilter.Configurations;

namespace BloomFilter;

/// <summary>
/// BloomFilter Builder
/// </summary>
public partial class FilterBuilder
{
    /// <summary>
    /// Creates a BloomFilter for the specified expected element
    /// </summary>
    /// <param name="options"><see cref="FilterMemoryOptions"/></param>
    /// <returns></returns>
    public static IBloomFilter Build(FilterMemoryOptions options)
    {
        return new FilterMemory(options);
    }

    /// <summary>
    /// Creates a BloomFilter for the specified expected element
    /// </summary>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(long expectedElements, string name = BloomFilterConstValue.DefaultInMemoryName)
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
    public static IBloomFilter Build(long expectedElements, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultInMemoryName)
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
    public static IBloomFilter Build(long expectedElements, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultInMemoryName)
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
    public static IBloomFilter Build(long expectedElements, double errorRate, string name = BloomFilterConstValue.DefaultInMemoryName)
    {
        return Build(expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3], name);
    }

    /// <summary>
    /// Creates a BloomFilter for the specified expected element
    /// </summary>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashMethod">The hash method.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IBloomFilter Build(long expectedElements, double errorRate, HashMethod hashMethod, string name = BloomFilterConstValue.DefaultInMemoryName)
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
    public static IBloomFilter Build(long expectedElements, double errorRate, HashFunction hashFunction, string name = BloomFilterConstValue.DefaultInMemoryName)
    {
        return new FilterMemory(name, expectedElements, errorRate, hashFunction);
    }
}