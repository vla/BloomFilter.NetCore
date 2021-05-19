using System;
using System.Collections.Generic;

namespace BloomFilter.Configurations
{
    /// <summary>
    /// BloomFilterOptions
    /// </summary>
    public class BloomFilterOptions
    {
        /// <summary>
        /// Gets the extensions.
        /// </summary>
        /// <value>The extensions.</value>
        internal IList<IBloomFilterOptionsExtension> Extensions { get; } = new List<IBloomFilterOptionsExtension>();

        /// <summary>
        /// Registers the extension.
        /// </summary>
        /// <param name="extension">Extension.</param>
        public void RegisterExtension(IBloomFilterOptionsExtension extension)
        {
            if (extension == null) throw new ArgumentNullException(nameof(extension));
            Extensions.Add(extension);
        }
    }
}