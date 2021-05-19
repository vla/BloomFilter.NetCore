using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BloomFilter.Configurations
{
    public class FilterMemoryOptionsExtension : IBloomFilterOptionsExtension
    {
        private readonly FilterMemoryOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMemoryOptionsExtension"/> class.
        /// </summary>
        /// <param name="options">Configure.</param>
        public FilterMemoryOptionsExtension(FilterMemoryOptions options)
        {
            _options = options;
        }

        public void AddServices(IServiceCollection services)
        {
            services.TryAddSingleton<IBloomFilterFactory, DefaultBloomFilterFactory>();
            services.AddSingleton<IBloomFilter, FilterMemory>(x =>
            {
                return new FilterMemory(
                    _options.Name,
                    _options.ExpectedElements,
                    _options.ErrorRate,
                    HashFunction.Functions[_options.Method]);
            });
        }
    }
}