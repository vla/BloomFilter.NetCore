using CSRedis;
using System;

namespace BloomFilter.CSRedis;

/// <summary>
/// BloomFilter CSRedis Builder - Supports both static factory methods and fluent API
/// </summary>
/// <example>
/// Fluent API:
/// <code>
/// var filter = FilterCSRedisBuilder.Create()
///     .WithRedisClient(csredisClient)
///     .WithRedisKey("bloom:users")
///     .WithName("UserFilter")
///     .ExpectingElements(10_000_000)
///     .WithErrorRate(0.001)
///     .UsingHashMethod(HashMethod.XXHash3)
///     .BuildCSRedis();
/// </code>
/// </example>
public class FilterCSRedisBuilder : FilterBuilder
{
    #region Fluent API - Instance Members

    private string? _redisKey;
    private CSRedisClient? _client;

    /// <summary>
    /// Sets the Redis key for the Bloom Filter
    /// </summary>
    /// <param name="redisKey">The Redis key</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentException">Thrown when redisKey is null or whitespace</exception>
    public FilterCSRedisBuilder WithRedisKey(string redisKey)
    {
        if (string.IsNullOrWhiteSpace(redisKey))
            throw new ArgumentException("Redis key cannot be null or whitespace", nameof(redisKey));
        _redisKey = redisKey;
        return this;
    }

    /// <summary>
    /// Sets the CSRedis client
    /// </summary>
    /// <param name="client">The CSRedisClient instance</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when client is null</exception>
    public FilterCSRedisBuilder WithRedisClient(CSRedisClient client)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(client);
#else
        if (client == null)
            throw new ArgumentNullException(nameof(client));
#endif
        _client = client;
        return this;
    }

    /// <summary>
    /// Builds a CSRedis-backed Bloom Filter with the configured settings
    /// </summary>
    /// <returns>A new IBloomFilter instance backed by CSRedis</returns>
    /// <exception cref="InvalidOperationException">Thrown when required configuration is missing</exception>
    public IBloomFilter BuildCSRedis()
    {
        if (string.IsNullOrWhiteSpace(_redisKey))
            throw new InvalidOperationException("Redis key must be specified using WithRedisKey()");

        if (_client == null)
            throw new InvalidOperationException("CSRedis client must be specified using WithRedisClient()");

        // Get hash function from base class configuration
        var hashFunction = _customHashFunction ?? HashFunction.Functions[_method];

        return new FilterCSRedis(_name, _client, _redisKey!, _expectedElements, _errorRate, hashFunction);
    }

    /// <summary>
    /// Builds a CSRedis-backed Bloom Filter with explicit capacity and hash count (advanced usage)
    /// </summary>
    /// <param name="capacity">The bit array capacity</param>
    /// <param name="hashes">The number of hash functions</param>
    /// <returns>A new IBloomFilter instance backed by CSRedis</returns>
    /// <exception cref="InvalidOperationException">Thrown when required configuration is missing</exception>
    public IBloomFilter BuildCSRedisWithCapacity(long capacity, int hashes)
    {
        if (string.IsNullOrWhiteSpace(_redisKey))
            throw new InvalidOperationException("Redis key must be specified using WithRedisKey()");

        if (_client == null)
            throw new InvalidOperationException("CSRedis client must be specified using WithRedisClient()");

        // Get hash function from base class configuration
        var hashFunction = _customHashFunction ?? HashFunction.Functions[_method];

        return new FilterCSRedis(_name, _client, _redisKey!, capacity, hashes, hashFunction);
    }

    #endregion
}
