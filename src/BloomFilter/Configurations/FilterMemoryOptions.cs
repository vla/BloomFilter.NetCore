using System.Collections;
using System.Collections.Generic;

namespace BloomFilter.Configurations;

public class FilterMemoryOptions
{
    /// <summary>
    /// The Name
    /// </summary>
    public string Name { get; set; } = BloomFilterConstValue.DefaultInMemoryName;

    /// <summary>
    /// The expected elements
    /// </summary>
    public long ExpectedElements { get; set; } = 1000000;

    /// <summary>
    /// The error rate
    /// </summary>
    public double ErrorRate { get; set; } = 0.01;

    /// <summary>
    /// The Hash Method
    /// </summary>
    public HashMethod Method { get; set; } = HashMethod.Murmur3;

    /// <summary>
    /// Multiple bitmap
    /// </summary>
    public BitArray[]? Buckets { get; set; }

    /// <summary>
    /// Multiple bitmap from bytes
    /// </summary>
    public IList<byte[]>? BucketBytes { get; set; }
}