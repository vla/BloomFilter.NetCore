using System.Collections;

namespace BloomFilter;

/// <summary>
/// FilterMemorySerializerParam
/// </summary>
public class FilterMemorySerializerParam
{
    /// <summary>
    /// Gets the name specified by BloomFilter.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// <see cref="HashMethod"/>
    /// </summary>
    public HashMethod Method { get; set; }

    /// <summary>
    ///  the expected elements.
    /// </summary>
    public long ExpectedElements { get; set; }

    /// <summary>
    /// the number of expected elements
    /// </summary>
    public double ErrorRate { get; set; }

    /// <summary>
    /// bitmap
    /// </summary>
    public BitArray[] Buckets { get; set; } = default!;
}
