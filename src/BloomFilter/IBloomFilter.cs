namespace BloomFilter
{
    /// <summary>
    /// Represents a Bloom filter.
    /// </summary>
    public interface IBloomFilter
    {
        /// <summary>
        /// Adds the passed value to the filter.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        bool Add(byte[] element);

        /// <summary>
        /// Tests whether an element is present in the filter
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        bool Contains(byte[] element);

        /// <summary>
        /// Removes all elements from the filter
        /// </summary>
        void Clear();

        /// <summary>
        /// Dispatches the hash function for a string value
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int[] ComputeHash(byte[] data);
    }
}