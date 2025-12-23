using System;

namespace BloomFilter;

/// <summary>
/// Fluent API Builder for creating Bloom Filters with a more expressive syntax
/// </summary>
/// <example>
/// <code>
/// var filter = FluentFilterBuilder.Create()
///     .WithName("UserFilter")
///     .ExpectingElements(10_000_000)
///     .WithErrorRate(0.001)
///     .UsingHashMethod(HashMethod.XXHash3)
///     .BuildInMemory();
/// </code>
/// </example>
public sealed class FluentFilterBuilder
{
    private string _name = BloomFilterConstValue.DefaultInMemoryName;
    private long _expectedElements = 1_000_000;
    private double _errorRate = 0.01;
    private HashMethod _method = HashMethod.Murmur3;
    private HashFunction? _customHashFunction;
    private IFilterMemorySerializer? _customSerializer;

    private FluentFilterBuilder() { }

    /// <summary>
    /// Creates a new Fluent Builder for Bloom Filter construction
    /// </summary>
    /// <returns>A new FluentFilterBuilder instance</returns>
    public static FluentFilterBuilder Create() => new();

    /// <summary>
    /// Sets the name for the Bloom Filter
    /// </summary>
    /// <param name="name">The filter name</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or whitespace</exception>
    public FluentFilterBuilder WithName(string name)
    {
#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
#else
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace", nameof(name));
#endif
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the expected number of elements to be added to the filter
    /// </summary>
    /// <param name="count">The expected element count (must be > 0)</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when count is less than or equal to 0</exception>
    public FluentFilterBuilder ExpectingElements(long count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), count, "Expected elements must be greater than 0");
        _expectedElements = count;
        return this;
    }

    /// <summary>
    /// Sets the acceptable false positive rate (probability)
    /// </summary>
    /// <param name="rate">The error rate (must be between 0 and 1, exclusive)</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when rate is not between 0 and 1</exception>
    public FluentFilterBuilder WithErrorRate(double rate)
    {
        if (rate <= 0 || rate >= 1)
            throw new ArgumentOutOfRangeException(nameof(rate), rate, "Error rate must be between 0 and 1 (exclusive)");
        _errorRate = rate;
        return this;
    }

    /// <summary>
    /// Sets the hash method to use for hashing elements
    /// </summary>
    /// <param name="method">The hash method</param>
    /// <returns>The builder for method chaining</returns>
    public FluentFilterBuilder UsingHashMethod(HashMethod method)
    {
        _method = method;
        _customHashFunction = null;
        return this;
    }

    /// <summary>
    /// Uses a custom hash function for hashing elements
    /// </summary>
    /// <param name="hashFunction">The custom hash function</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when hashFunction is null</exception>
    public FluentFilterBuilder UsingCustomHash(HashFunction hashFunction)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(hashFunction);
#else
        if (hashFunction == null)
            throw new ArgumentNullException(nameof(hashFunction));
#endif
        _customHashFunction = hashFunction;
        return this;
    }

    /// <summary>
    /// Uses a custom serializer for persisting the Bloom Filter
    /// </summary>
    /// <param name="serializer">The custom serializer</param>
    /// <returns>The builder for method chaining</returns>
    /// <exception cref="ArgumentNullException">Thrown when serializer is null</exception>
    public FluentFilterBuilder WithSerializer(IFilterMemorySerializer serializer)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(serializer);
#else
        if (serializer == null)
            throw new ArgumentNullException(nameof(serializer));
#endif
        _customSerializer = serializer;
        return this;
    }

    /// <summary>
    /// Builds an in-memory Bloom Filter with the configured settings
    /// </summary>
    /// <returns>A new IBloomFilter instance</returns>
    public IBloomFilter BuildInMemory()
    {
        var hashFunction = _customHashFunction ?? HashFunction.Functions[_method];
        var serializer = _customSerializer ?? new DefaultFilterMemorySerializer();

        return new FilterMemory(_name, _expectedElements, _errorRate, hashFunction, serializer);
    }

    /// <summary>
    /// Builds an in-memory Bloom Filter with explicit capacity and hash count (advanced usage)
    /// </summary>
    /// <param name="capacity">The bit array capacity</param>
    /// <param name="hashes">The number of hash functions</param>
    /// <returns>A new IBloomFilter instance</returns>
    public IBloomFilter BuildInMemoryWithCapacity(long capacity, int hashes)
    {
        var hashFunction = _customHashFunction ?? HashFunction.Functions[_method];
        var serializer = _customSerializer ?? new DefaultFilterMemorySerializer();

        return new FilterMemory(_name, capacity, hashes, hashFunction, serializer);
    }
}
