﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloomFilter;

/// <summary>
/// Represents a Bloom filter and provides
/// </summary>
/// <seealso cref="IBloomFilter" />
public abstract class Filter : IBloomFilter
{
    //256MB
    protected const int MaxInt = 2147483640;

    /// <summary>
    /// Gets the name specified by BloomFilter.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// <see cref="HashFunction"/>
    /// </summary>
    public HashFunction Hash { get; private set; }

    /// <summary>
    /// the Capacity of the Bloom filter
    /// </summary>
    public long Capacity { get; private set; }

    /// <summary>
    /// number of hash functions
    /// </summary>
    public int Hashes { get; private set; }

    /// <summary>
    ///  the expected elements.
    /// </summary>
    public long ExpectedElements { get; private set; }

    /// <summary>
    /// the number of expected elements
    /// </summary>
    public double ErrorRate { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Filter"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// expectedElements - expectedElements must be > 0
    /// or
    /// errorRate
    /// </exception>
    /// <exception cref="ArgumentNullException">hashFunction</exception>
    public Filter(string name, long expectedElements, double errorRate, HashFunction hashFunction)
    {
        if (expectedElements < 1)
            throw new ArgumentOutOfRangeException("expectedElements", expectedElements, "expectedElements must be > 0");
        if (errorRate >= 1 || errorRate <= 0)
            throw new ArgumentOutOfRangeException("errorRate", errorRate, string.Format("errorRate must be between 0 and 1, exclusive. Was {0}", errorRate));

        Name = name;
        ExpectedElements = expectedElements;
        ErrorRate = errorRate;
        Hash = hashFunction ?? throw new ArgumentNullException(nameof(hashFunction));

        Capacity = BestM(expectedElements, errorRate);
        Hashes = BestK(expectedElements, Capacity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Filter"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="capacity">The capacity.</param>
    /// <param name="hashes">The hashes.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// capacity - capacity must be > 0
    /// or
    /// hashes - hashes must be > 0
    /// </exception>
    /// <exception cref="ArgumentNullException">hashFunction</exception>
    public Filter(string name, long capacity, int hashes, HashFunction hashFunction)
    {
        if (capacity < 1)
            throw new ArgumentOutOfRangeException("capacity", capacity, "capacity must be > 0");
        if (hashes < 1)
            throw new ArgumentOutOfRangeException("hashes", hashes, "hashes must be > 0");

        Name = name;
        Capacity = capacity;
        Hashes = hashes;
        Hash = hashFunction ?? throw new ArgumentNullException(nameof(hashFunction));

        ExpectedElements = BestN(hashes, capacity);
        ErrorRate = BestP(hashes, capacity, ExpectedElements);
    }

    protected void SetFilterParam(long expectedElements, double errorRate, HashMethod method)
    {
        if (expectedElements < 1)
            throw new ArgumentOutOfRangeException("expectedElements", expectedElements, "expectedElements must be > 0");
        if (errorRate >= 1 || errorRate <= 0)
            throw new ArgumentOutOfRangeException("errorRate", errorRate, string.Format("errorRate must be between 0 and 1, exclusive. Was {0}", errorRate));

        ExpectedElements = expectedElements;
        ErrorRate = errorRate;
        Hash = HashFunction.Functions[method];

        Capacity = BestM(expectedElements, errorRate);
        Hashes = BestK(expectedElements, Capacity);
    }

    /// <summary>
    /// Adds the passed value to the filter.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public abstract bool Add(ReadOnlySpan<byte> data);

    /// <summary>
    /// Async Adds the passed value to the filter.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public abstract ValueTask<bool> AddAsync(ReadOnlyMemory<byte> data);

    /// <summary>
    /// Adds the specified elements.
    /// </summary>
    /// <param name="elements">The elements.</param>
    /// <returns></returns>
    public abstract IList<bool> Add(IEnumerable<byte[]> elements);

    /// <summary>
    /// Async Adds the specified elements.
    /// </summary>
    /// <param name="elements">The elements.</param>
    /// <returns></returns>
    public abstract ValueTask<IList<bool>> AddAsync(IEnumerable<byte[]> elements);

    /// <summary>
    /// Removes all elements from the filter
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// Async Removes all elements from the filter
    /// </summary>
    public abstract ValueTask ClearAsync();

    /// <summary>
    /// Tests whether an element is present in the filter
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public abstract bool Contains(ReadOnlySpan<byte> element);

    /// <summary>
    /// Async Tests whether an element is present in the filter
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public abstract ValueTask<bool> ContainsAsync(ReadOnlyMemory<byte> element);

    /// <summary>
    /// Tests whether an elements is present in the filter
    /// </summary>
    /// <param name="elements"></param>
    /// <returns></returns>
    public abstract IList<bool> Contains(IEnumerable<byte[]> elements);

    /// <summary>
    /// Async Tests whether an elements is present in the filter
    /// </summary>
    /// <param name="elements"></param>
    /// <returns></returns>
    public abstract ValueTask<IList<bool>> ContainsAsync(IEnumerable<byte[]> elements);

    /// <summary>
    /// Alls the specified elements.
    /// </summary>
    /// <param name="elements">The elements.</param>
    /// <returns></returns>
    public abstract bool All(IEnumerable<byte[]> elements);

    /// <summary>
    /// Async Alls the specified elements.
    /// </summary>
    /// <param name="elements">The elements.</param>
    /// <returns></returns>
    public abstract ValueTask<bool> AllAsync(IEnumerable<byte[]> elements);

    /// <summary>
    ///  Hashes the specified value.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public long[] ComputeHash(ReadOnlySpan<byte> data)
    {
        return Hash.ComputeHash(data, Capacity, Hashes);
    }

    /// <summary>
    /// Calculates the optimal size of the bloom filter in bits given expectedElements (expected
    /// number of elements in bloom filter) and falsePositiveProbability (tolerable false positive rate).
    /// </summary>
    /// <param name="n">Expected number of elements inserted in the bloom filter</param>
    /// <param name="p">Tolerable false positive rate</param>
    /// <returns>the optimal siz of the bloom filter in bits</returns>
    public static long BestM(long n, double p)
    {
        return (long)Math.Ceiling(-1 * (n * Math.Log(p)) / Math.Pow(Math.Log(2), 2));
    }

    /// <summary>
    /// Calculates the optimal hashes (number of hash function) given expectedElements (expected number of
    /// elements in bloom filter) and size (size of bloom filter in bits).
    /// </summary>
    /// <param name="n">Expected number of elements inserted in the bloom filter</param>
    /// <param name="m">The size of the bloom filter in bits.</param>
    /// <returns>the optimal amount of hash functions hashes</returns>
    public static int BestK(long n, long m)
    {
        return (int)Math.Ceiling((Math.Log(2) * m) / n);
    }

    /// <summary>
    /// Calculates the amount of elements a Bloom filter for which the given configuration of size and hashes is optimal.
    /// </summary>
    /// <param name="k">number of hashes</param>
    /// <param name="m">The size of the bloom filter in bits.</param>
    /// <returns>mount of elements a Bloom filter for which the given configuration of size and hashes is optimal</returns>
    public static long BestN(int k, long m)
    {
        return (long)Math.Ceiling((Math.Log(2) * m) / k);
    }

    /// <summary>
    /// Calculates the best-case (uniform hash function) false positive probability.
    /// </summary>
    /// <param name="k"> number of hashes</param>
    /// <param name="m">The size of the bloom filter in bits.</param>
    /// <param name="insertedElements">number of elements inserted in the filter</param>
    /// <returns>The calculated false positive probability</returns>
    public static double BestP(int k, long m, double insertedElements)
    {
        return Math.Pow((1 - Math.Exp(-k * insertedElements / m)), k);
    }

    public override string ToString()
    {
        return $"Capacity:{Capacity},Hashes:{Hashes},ExpectedElements:{ExpectedElements},ErrorRate:{ErrorRate}";
    }

    protected static int LogMaxInt(long number, out int mod)
    {
        mod = (int)(number % MaxInt);
        return (int)(number / MaxInt);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public abstract void Dispose();
}
