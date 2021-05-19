namespace BloomFilter.Redis
{
    /// <summary>
    /// Bloom Filter Redis Implement
    /// </summary>
#pragma warning disable CS0618
    public class FilterRedis : FilterRedis<byte[]>
#pragma warning restore CS0618
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRedis"/> class.
        /// </summary>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="name">The name.</param>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterRedis(IRedisBitOperate redisBitOperate, string name, int expectedElements, double errorRate, HashFunction hashFunction)
            : base(redisBitOperate, name, expectedElements, errorRate, hashFunction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRedis"/> class.
        /// </summary>
        /// <param name="redisBitOperate">The redis bit operate.</param>
        /// <param name="name">The name.</param>
        /// <param name="capacity">The capacity.</param>
        /// <param name="hashes">The hashes.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterRedis(IRedisBitOperate redisBitOperate, string name, int capacity, int hashes, HashFunction hashFunction)
             : base(redisBitOperate, name, capacity, hashes, hashFunction)
        {
        }
    }
}