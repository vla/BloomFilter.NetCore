using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloomFilter;

/// <summary>
/// Abstract base class for Redis-based Bloom Filter implementations.
/// Encapsulates common batch operation logic shared across all Redis implementations.
/// </summary>
public abstract class FilterRedisBase : Filter
{
    protected FilterRedisBase(string name, long expectedElements, double errorRate, HashFunction hashFunction)
        : base(name, expectedElements, errorRate, hashFunction)
    {
    }

    protected FilterRedisBase(string name, long capacity, int hashes, HashFunction hashFunction)
        : base(name, capacity, hashes, hashFunction)
    {
    }

    // Abstract methods - to be implemented by concrete Redis client implementations

    /// <summary>
    /// Sets a bit at the specified position in the Redis bitmap.
    /// </summary>
    /// <param name="position">The bit position.</param>
    /// <returns>The previous value of the bit (true if it was set, false otherwise).</returns>
    protected abstract bool SetBit(long position);

    /// <summary>
    /// Gets the bit value at the specified position in the Redis bitmap.
    /// </summary>
    /// <param name="position">The bit position.</param>
    /// <returns>The value of the bit (true if set, false otherwise).</returns>
    protected abstract bool GetBit(long position);

    /// <summary>
    /// Asynchronously sets a bit at the specified position in the Redis bitmap.
    /// </summary>
    /// <param name="position">The bit position.</param>
    /// <returns>The previous value of the bit.</returns>
    protected abstract Task<bool> SetBitAsync(long position);

    /// <summary>
    /// Asynchronously gets the bit value at the specified position in the Redis bitmap.
    /// </summary>
    /// <param name="position">The bit position.</param>
    /// <returns>The value of the bit.</returns>
    protected abstract Task<bool> GetBitAsync(long position);

    /// <summary>
    /// Sets multiple bits at the specified positions in the Redis bitmap.
    /// </summary>
    /// <param name="positions">Array of bit positions.</param>
    /// <returns>Array of previous bit values.</returns>
    protected abstract bool[] SetBits(long[] positions);

    /// <summary>
    /// Gets multiple bits at the specified positions in the Redis bitmap.
    /// </summary>
    /// <param name="positions">Array of bit positions.</param>
    /// <returns>Array of bit values.</returns>
    protected abstract bool[] GetBits(long[] positions);

    /// <summary>
    /// Asynchronously sets multiple bits at the specified positions in the Redis bitmap.
    /// </summary>
    /// <param name="positions">Array of bit positions.</param>
    /// <returns>Array of previous bit values.</returns>
    protected abstract Task<bool[]> SetBitsAsync(long[] positions);

    /// <summary>
    /// Asynchronously gets multiple bits at the specified positions in the Redis bitmap.
    /// </summary>
    /// <param name="positions">Array of bit positions.</param>
    /// <returns>Array of bit values.</returns>
    protected abstract Task<bool[]> GetBitsAsync(long[] positions);

    /// <summary>
    /// Clears all bits in the Redis bitmap.
    /// </summary>
    protected abstract void ClearBits();

    /// <summary>
    /// Asynchronously clears all bits in the Redis bitmap.
    /// </summary>
    protected abstract Task ClearBitsAsync();

    // Common implementations - shared by all Redis implementations

    public override IList<bool> Add(IEnumerable<byte[]> elements)
    {
        // Pre-allocate capacity if possible
        var elementsList = elements as IList<byte[]> ?? elements.ToList();
        var hashes = new List<long>(elementsList.Count * Hashes);

        foreach (var element in elementsList)
        {
            hashes.AddRange(ComputeHash(element));
        }

        var processResults = SetBits(hashes.ToArray());
        return ProcessBatchResults(processResults, Hashes, trackAdded: true);
    }

    public override async ValueTask<IList<bool>> AddAsync(IEnumerable<byte[]> elements)
    {
        // Pre-allocate capacity if possible
        var elementsList = elements as IList<byte[]> ?? elements.ToList();
        var hashes = new List<long>(elementsList.Count * Hashes);

        foreach (var element in elementsList)
        {
            hashes.AddRange(ComputeHash(element));
        }

        var processResults = await SetBitsAsync(hashes.ToArray());
        return ProcessBatchResults(processResults, Hashes, trackAdded: true);
    }

    public override IList<bool> Contains(IEnumerable<byte[]> elements)
    {
        // Pre-allocate capacity if possible
        var elementsList = elements as IList<byte[]> ?? elements.ToList();
        var hashes = new List<long>(elementsList.Count * Hashes);

        foreach (var element in elementsList)
        {
            hashes.AddRange(ComputeHash(element));
        }

        var processResults = GetBits(hashes.ToArray());
        return ProcessBatchResults(processResults, Hashes, trackAdded: false);
    }

    public override async ValueTask<IList<bool>> ContainsAsync(IEnumerable<byte[]> elements)
    {
        // Pre-allocate capacity if possible
        var elementsList = elements as IList<byte[]> ?? elements.ToList();
        var hashes = new List<long>(elementsList.Count * Hashes);

        foreach (var element in elementsList)
        {
            hashes.AddRange(ComputeHash(element));
        }

        var processResults = await GetBitsAsync(hashes.ToArray());
        return ProcessBatchResults(processResults, Hashes, trackAdded: false);
    }

    public override bool All(IEnumerable<byte[]> elements)
        => Contains(elements).All(e => e);

    public override ValueTask<bool> AllAsync(IEnumerable<byte[]> elements)
        => new(Contains(elements).All(e => e));

    public override void Clear()
        => ClearBits();

    public override async ValueTask ClearAsync()
        => await ClearBitsAsync();

    /// <summary>
    /// Processes batch operation results, converting bit-level results to element-level results.
    /// </summary>
    /// <param name="processResults">Bit-level operation results (from Redis).</param>
    /// <param name="hashes">Number of hash functions (bits per element).</param>
    /// <param name="trackAdded">If true, tracks additions (wasAdded logic); if false, tracks presence (isPresent logic).</param>
    /// <returns>Element-level results.</returns>
    protected static IList<bool> ProcessBatchResults(bool[] processResults, int hashes, bool trackAdded)
    {
        var results = new List<bool>(processResults.Length / hashes);
        bool flag = trackAdded ? false : true;
        int processed = 0;

        foreach (var item in processResults)
        {
            if (trackAdded && !item) flag = true;      // Add: any bit not previously set = new addition
            if (!trackAdded && !item) flag = false;    // Contains: any bit not set = not present

            if ((processed + 1) % hashes == 0)
            {
                results.Add(flag);
                flag = trackAdded ? false : true;
            }
            processed++;
        }

        return results;
    }
}
