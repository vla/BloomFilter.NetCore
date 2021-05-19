using Microsoft.Extensions.DependencyInjection;

namespace BloomFilter.Configurations
{
    /// <summary>
    /// BloomFilter options extension.
    /// </summary>
    public interface IBloomFilterOptionsExtension
    {
        /// <summary>
        /// Adds the services.
        /// </summary>
        /// <param name="services">Services.</param>
        void AddServices(IServiceCollection services);
    }
}