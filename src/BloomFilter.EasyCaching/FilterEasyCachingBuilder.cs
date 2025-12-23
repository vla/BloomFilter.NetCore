using EasyCaching.Core;
using System;

namespace BloomFilter.EasyCaching;

/// <summary>
/// BloomFilter EasyCaching Builder - Supports both static factory methods and fluent API
/// </summary>
/// <example>
/// Fluent API:
/// <code>
/// var filter = FilterEasyCachingBuilder.Create()
///     .WithRedisCachingProvider(provider)
///     .WithRedisKey("bloom:users")
///     .WithName("UserFilter")
///     .ExpectingElements(10_000_000)
///     .WithErrorRate(0.001)
///     .UsingHashMethod(HashMethod.XXHash3)
///     .BuildEasyCaching();
/// </code>
/// </example>
public class FilterEasyCachingBuilder : FilterBuilder
{
    #region Fluent API - Instance Members

    private string? _redisKey;
    private IRedisCachingProvider? _provider;

    /// <summary>
    /// Sets the Redis key for the Bloom Filter
    /// </summary>
    /// <param name="redisKey">The Redis key</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentException">Thrown when redisKey is null or whitespace</exception>
    public FilterEasyCachingBuilder WithRedisKey(string redisKey)
    {
        if (string.IsNullOrWhiteSpace(redisKey))
            throw new ArgumentException("Redis key cannot be null or whitespace", nameof(redisKey));
        _redisKey = redisKey;
        return this;
    }

    /// <summary>
    /// Sets the EasyCaching Redis caching provider
    /// </summary>
    /// <param name="provider">The IRedisCachingProvider instance</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when provider is null</exception>
    public FilterEasyCachingBuilder WithRedisCachingProvider(IRedisCachingProvider provider)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(provider);
#else
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));
#endif
        _provider = provider;
        return this;
    }

    /// <summary>
    /// Builds an EasyCaching-backed Bloom Filter with the configured settings
    /// </summary>
    /// <returns>A new IBloomFilter instance backed by EasyCaching</returns>
    /// <exception cref="InvalidOperationException">Thrown when required configuration is missing</exception>
    public IBloomFilter BuildEasyCaching()
    {
        if (string.IsNullOrWhiteSpace(_redisKey))
            throw new InvalidOperationException("Redis key must be specified using WithRedisKey()");

        if (_provider == null)
            throw new InvalidOperationException("Redis caching provider must be specified using WithRedisCachingProvider()");

        // Get hash function from base class configuration
        var hashFunction = _customHashFunction ?? HashFunction.Functions[_method];

        return new FilterEasyCachingRedis(_name, _provider, _redisKey!, _expectedElements, _errorRate, hashFunction);
    }

    /// <summary>
    /// Builds an EasyCaching-backed Bloom Filter with explicit capacity and hash count (advanced usage)
    /// </summary>
    /// <param name="capacity">The bit array capacity</param>
    /// <param name="hashes">The number of hash functions</param>
    /// <returns>A new IBloomFilter instance backed by EasyCaching</returns>
    /// <exception cref="InvalidOperationException">Thrown when required configuration is missing</exception>
    public IBloomFilter BuildEasyCachingWithCapacity(long capacity, int hashes)
    {
        if (string.IsNullOrWhiteSpace(_redisKey))
            throw new InvalidOperationException("Redis key must be specified using WithRedisKey()");

        if (_provider == null)
            throw new InvalidOperationException("Redis caching provider must be specified using WithRedisCachingProvider()");

        // Get hash function from base class configuration
        var hashFunction = _customHashFunction ?? HashFunction.Functions[_method];

        return new FilterEasyCachingRedis(_name, _provider, _redisKey!, capacity, hashes, hashFunction);
    }

    #endregion
}
