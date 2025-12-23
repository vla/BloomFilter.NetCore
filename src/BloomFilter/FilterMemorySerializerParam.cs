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
#if NET5_0_OR_GREATER
    public string Name { get; init; } = default!;
#else
    public string Name { get; set; } = default!;
#endif

    /// <summary>
    /// <see cref="HashMethod"/>
    /// </summary>
#if NET5_0_OR_GREATER
    public HashMethod Method { get; init; }
#else
    public HashMethod Method { get; set; }
#endif

    /// <summary>
    ///  the expected elements.
    /// </summary>
#if NET5_0_OR_GREATER
    public long ExpectedElements { get; init; }
#else
    public long ExpectedElements { get; set; }
#endif

    /// <summary>
    /// the number of expected elements
    /// </summary>
#if NET5_0_OR_GREATER
    public double ErrorRate { get; init; }
#else
    public double ErrorRate { get; set; }
#endif

    /// <summary>
    /// bitmap
    /// </summary>
#if NET5_0_OR_GREATER
    public BitArray[] Buckets { get; init; } = default!;
#else
    public BitArray[] Buckets { get; set; } = default!;
#endif
}
