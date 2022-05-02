using System.Diagnostics.CodeAnalysis;

namespace BloomFilter
{
    /// <summary>
    /// BloomFilter Factory
    /// </summary>
    public interface IBloomFilterFactory
    {
        /// <summary>
        /// Gets the <see cref="IBloomFilter"/>
        /// </summary>
        /// <returns>The bloomFilter.</returns>
        /// <param name="name">Name.</param>
        IBloomFilter Get(string name);

        /// <summary>
        /// Try Gets the<see cref="IBloomFilter"/>
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="bloomFilter">The bloomFilter</param>
        /// <returns></returns>
        bool TryGet(string name,
#if NET5_0_OR_GREATER
            [MaybeNullWhen(false)]
#endif
        out IBloomFilter bloomFilter);
    }
}