using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloomFilter;

/// <summary>
/// Bloom Filter In Mempory Implement
/// </summary>
public class FilterMemory : Filter
{
    private const int MaxInt = 2147483647;

    //MAX 512MB
    private readonly BitArray _hashBits1;

    private readonly BitArray? _hashBits2;

    private readonly object sync = new();

    private static readonly ValueTask Empty = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterMemory"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashFunction">The hash function.</param>
    public FilterMemory(string name, uint expectedElements, double errorRate, HashFunction hashFunction)
        : base(name, expectedElements, errorRate, hashFunction)
    {
        if (Capacity > MaxInt)
        {
            _hashBits1 = new BitArray(MaxInt);
            _hashBits2 = new BitArray((int)(Capacity - MaxInt));
        }
        else
        {
            _hashBits1 = new BitArray((int)Capacity);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterMemory"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="size">The size.</param>
    /// <param name="hashes">The hashes.</param>
    /// <param name="hashFunction">The hash function.</param>
    public FilterMemory(string name, uint size, uint hashes, HashFunction hashFunction)
        : base(name, size, hashes, hashFunction)
    {
        if (Capacity > MaxInt)
        {
            _hashBits1 = new BitArray(MaxInt);
            _hashBits2 = new BitArray((int)(Capacity - MaxInt));
        }
        else
        {
            _hashBits1 = new BitArray((int)Capacity);
        }
    }

    /// <summary>
    /// Adds the passed value to the filter.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override bool Add(ReadOnlySpan<byte> data)
    {
        bool added = false;
        var positions = ComputeHash(data);
        lock (sync)
        {
            foreach (var position in positions)
            {
                if (!Get(position))
                {
                    added = true;
                    Set(position);
                }
            }
        }
        return added;
    }

    /// <summary>
    /// Adds the passed value to the filter.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public override ValueTask<bool> AddAsync(ReadOnlyMemory<byte> data)
    {
        return new ValueTask<bool>(Add(data.Span));
    }

    public override IList<bool> Add(IEnumerable<byte[]> elements)
    {
        var hashes = new List<uint>();
        foreach (var element in elements)
        {
            hashes.AddRange(ComputeHash(element));
        }

        var processResults = new bool[hashes.Count];
        lock (sync)
        {
            for (var i = 0; i < hashes.Count; i++)
            {
                if (!Get(hashes[i]))
                {
                    Set(hashes[i]);
                    processResults[i] = false;
                }
                else
                {
                    processResults[i] = true;
                }
            }
        }

        IList<bool> results = new List<bool>();
        bool wasAdded = false;
        int processed = 0;

        //For each value check, if all bits in ranges of hashes bits are set
        foreach (var item in processResults)
        {
            if (!item) wasAdded = true;
            if ((processed + 1) % Hashes == 0)
            {
                results.Add(wasAdded);
                wasAdded = false;
            }
            processed++;
        }

        return results;
    }

    public override ValueTask<IList<bool>> AddAsync(IEnumerable<byte[]> elements)
    {
        return new ValueTask<IList<bool>>(Add(elements));
    }

    /// <summary>
    /// Tests whether an element is present in the filter
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public override bool Contains(ReadOnlySpan<byte> element)
    {
        var positions = ComputeHash(element);
        lock (sync)
        {
            foreach (var position in positions)
            {
                if (!Get(position))
                    return false;
            }
        }
        return true;
    }

    public override ValueTask<bool> ContainsAsync(ReadOnlyMemory<byte> element)
    {
        return new ValueTask<bool>(Contains(element.Span));
    }

    public override IList<bool> Contains(IEnumerable<byte[]> elements)
    {
        var hashes = new List<uint>();
        foreach (var element in elements)
        {
            hashes.AddRange(ComputeHash(element));
        }

        var processResults = new bool[hashes.Count];
        lock (sync)
        {
            for (var i = 0; i < hashes.Count; i++)
            {
                processResults[i] = Get(hashes[i]);
            }
        }

        IList<bool> results = new List<bool>();
        bool isPresent = true;
        int processed = 0;

        //For each value check, if all bits in ranges of hashes bits are set
        foreach (var item in processResults)
        {
            if (!item) isPresent = false;
            if ((processed + 1) % Hashes == 0)
            {
                results.Add(isPresent);
                isPresent = true;
            }
            processed++;
        }

        return results;
    }

    public override ValueTask<IList<bool>> ContainsAsync(IEnumerable<byte[]> elements)
    {
        return new ValueTask<IList<bool>>(Contains(elements));
    }

    public override bool All(IEnumerable<byte[]> elements)
    {
        return Contains(elements).All(e => e);
    }

    public override ValueTask<bool> AllAsync(IEnumerable<byte[]> elements)
    {
        return new ValueTask<bool>(All(elements));
    }

    /// <summary>
    /// Removes all elements from the filter
    /// </summary>
    public override void Clear()
    {
        lock (sync)
        {
            _hashBits1.SetAll(false);
            _hashBits2?.SetAll(false);
        }
    }

    public override ValueTask ClearAsync()
    {
        Clear();
        return Empty;
    }

    private void Set(uint index)
    {
        if (_hashBits2 is not null && index > MaxInt)
        {
            _hashBits2.Set((int)(index - MaxInt), true);
        }
        else
        {
            _hashBits1.Set((int)index, true);
        }
    }

    public bool Get(uint index)
    {
        if (_hashBits2 is not null && index > MaxInt)
        {
            return _hashBits2.Get((int)(index - MaxInt));
        }
        else
        {
            return _hashBits1.Get((int)index);
        }
    }

    public override void Dispose()
    {
    }
}