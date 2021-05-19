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
        /// <param name="name"></param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterMemory(string name, int expectedElements, double errorRate, HashFunction hashFunction)
            : base(name, expectedElements, errorRate, hashFunction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMemory{T}"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size">The size.</param>
        /// <param name="hashes">The hashes.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterMemory(string name, int size, int hashes, HashFunction hashFunction)
            : base(name, size, hashes, hashFunction)
        {
        }
    }
}