namespace BloomFilter
{
    /// <summary>
    /// Bloom Filter In Mempory Implement
    /// </summary>
    /// <seealso cref="Filter" />
#pragma warning disable CS0618 
    public class FilterMemory : FilterMemory<byte[]>
#pragma warning restore CS0618
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMemory{T}"/> class.
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterMemory(int expectedElements, double errorRate, HashFunction hashFunction)
            : base(expectedElements, errorRate, hashFunction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMemory{T}"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="hashes">The hashes.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterMemory(int size, int hashes, HashFunction hashFunction)
            : base(size, hashes, hashFunction)
        {
        }
    }
}