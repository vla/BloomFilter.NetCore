using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BloomFilter.Configurations;

namespace BloomFilter;

/// <summary>
/// Bloom Filter In Mempory Implement
/// </summary>
public class FilterMemory : Filter
{
    //The upper limit per bucket is 2147483640
    private BitArray[] _buckets;

    private readonly AsyncLock _mutex = new();
    private readonly IFilterMemorySerializer _filterMemorySerializer;

    private static readonly ValueTask Empty = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterMemory"/> class.
    /// </summary>
    /// <param name="options"><see cref="FilterMemoryOptions"/></param>
    /// <param name="filterMemorySerializer"></param>
    public FilterMemory(FilterMemoryOptions options, IFilterMemorySerializer filterMemorySerializer)
        : base(options.Name, options.ExpectedElements, options.ErrorRate, HashFunction.Functions[options.Method])
    {
        _filterMemorySerializer = filterMemorySerializer;

        if (options.Buckets is not null)
        {
            Import(options.Buckets);
        }
        else if (options.BucketBytes is not null)
        {
            Import(options.BucketBytes);
        }
        else
        {
            Init();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterMemory"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="expectedElements">The expected elements.</param>
    /// <param name="errorRate">The error rate.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <param name="filterMemorySerializer"></param>
    public FilterMemory(string name, long expectedElements, double errorRate, HashFunction hashFunction, IFilterMemorySerializer filterMemorySerializer)
        : base(name, expectedElements, errorRate, hashFunction)
    {
        _filterMemorySerializer = filterMemorySerializer;

        Init();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterMemory"/> class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="size">The size.</param>
    /// <param name="hashes">The hashes.</param>
    /// <param name="hashFunction">The hash function.</param>
    /// <param name="filterMemorySerializer"></param>
    public FilterMemory(string name, long size, int hashes, HashFunction hashFunction, IFilterMemorySerializer filterMemorySerializer)
        : base(name, size, hashes, hashFunction)
    {
        _filterMemorySerializer = filterMemorySerializer;
        Init();
    }

    [MemberNotNull(nameof(_buckets))]
    private void Init()
    {
        var bits = new List<BitArray>();
        var m = Capacity;
        while (m > 0)
        {
            if (m > MaxInt)
            {
                bits.Add(new BitArray(MaxInt));
                m -= MaxInt;
            }
            else
            {
                bits.Add(new BitArray((int)m));
                break;
            }
        }
        _buckets = bits.ToArray();
    }

    /// <summary>
    /// Serialize to a stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public async ValueTask SerializeAsync(Stream stream)
    {
        using var _ = await _mutex.AcquireAsync();

        await _filterMemorySerializer.SerializeAsync(new FilterMemorySerializerParam
        {
            Name = Name,
            Method = Hash.Method,
            ExpectedElements = ExpectedElements,
            ErrorRate = ErrorRate,
            Buckets = _buckets.Select(s => new BitArray(s)).ToArray()
        }, stream);
    }

    /// <summary>
    /// Deserialize from the stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public async ValueTask DeserializeAsync(Stream stream)
    {
        using var _ = await _mutex.AcquireAsync();

        var param = await _filterMemorySerializer.DeserializeAsync(stream);

        if (param.Buckets is null)
            throw new ArgumentNullException(nameof(FilterMemorySerializerParam.Buckets));

        if (param.Buckets.Length == 0)
            throw new ArgumentOutOfRangeException($"The length must greater than 0", nameof(FilterMemorySerializerParam.Buckets));

        SetFilterParam(param.ExpectedElements, param.ErrorRate, param.Method);

        if (Capacity > param.Buckets.Sum(s => (long)s.Length))
        {
            throw new ArgumentOutOfRangeException($"The length must less than or equal to {Capacity}", nameof(FilterMemorySerializerParam.Buckets));
        }

        _buckets = new BitArray[param.Buckets.Length];

        for (int i = 0; i < param.Buckets.Length; i++)
        {
            _buckets[i] = new BitArray(param.Buckets[i]);
        }
    }

    /// <summary>
    /// Importing bitmap
    /// </summary>
    /// <param name="buckets">Sets the multiple bitmap</param>
    [MemberNotNull(nameof(_buckets))]
    public void Import(BitArray[] buckets)
    {
        if (buckets is null)
            throw new ArgumentNullException(nameof(buckets));

        if (buckets.Length == 0)
            throw new ArgumentOutOfRangeException($"The length must greater than 0", nameof(buckets));

        if (Capacity > buckets.Sum(s => (long)s.Length))
        {
            throw new ArgumentOutOfRangeException($"The length must less than or equal to {Capacity}", nameof(buckets));
        }

        using var _ = _mutex.Acquire();

        _buckets = new BitArray[buckets.Length];

        for (int i = 0; i < buckets.Length; i++)
        {
            _buckets[i] = new BitArray(buckets[i]);
        }
    }

    /// <summary>
    /// Importing bitmap
    /// </summary>
    /// <param name="bucketBytes">Sets the multiple bitmaps</param>
    [MemberNotNull(nameof(_buckets))]
    public void Import(IList<byte[]> bucketBytes)
    {
        if (bucketBytes is null)
            throw new ArgumentNullException(nameof(bucketBytes));

        if (bucketBytes.Count == 0)
            throw new ArgumentOutOfRangeException($"The length must greater than 0", nameof(bucketBytes));

        Import(bucketBytes.Select(s => new BitArray(s)).ToArray());
    }

    /// <summary>
    /// Exporting bitmap
    /// </summary>
    public BitArray[] Export()
    {
        using var _ = _mutex.Acquire();
        return _buckets.Select(s => new BitArray(s)).ToArray();
    }

    /// <summary>
    /// Exporting bitmap
    /// </summary>
    public IList<byte[]> ExportToBytes()
    {
        int Mod(int len) => len % 8 > 0 ? 1 : 0;

        var result = new List<byte[]>();

        using (var _ = _mutex.Acquire())
        {
            foreach (var bucket in _buckets)
            {
                var bits = new byte[bucket.Length / 8 + Mod(bucket.Length)];
                bucket.CopyTo(bits, 0);
                result.Add(bits);
            }
        }

        return result;
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
        using (var _ = _mutex.Acquire())
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
        var hashes = new List<long>();
        foreach (var element in elements)
        {
            hashes.AddRange(ComputeHash(element));
        }

        var processResults = new bool[hashes.Count];
        using (var _ = _mutex.Acquire())
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
        using (var _ = _mutex.Acquire())
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
        var hashes = new List<long>();
        foreach (var element in elements)
        {
            hashes.AddRange(ComputeHash(element));
        }

        var processResults = new bool[hashes.Count];
        using (var _ = _mutex.Acquire())
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
        using var _ = _mutex.Acquire();
        foreach (var item in _buckets)
        {
            item.SetAll(false);
        }
    }

    public override ValueTask ClearAsync()
    {
        Clear();
        return Empty;
    }

    private void Set(long index)
    {
        int idx = LogMaxInt(index, out int mod);

        _buckets[idx].Set(mod, true);
    }

    public bool Get(long index)
    {
        int idx = LogMaxInt(index, out int mod);
        return _buckets[idx].Get(mod);
    }

    public override void Dispose()
    {
    }
}