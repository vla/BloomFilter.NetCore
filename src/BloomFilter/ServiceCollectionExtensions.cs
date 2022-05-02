using BloomFilter;
using BloomFilter.Configurations;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the BloomFilter.
        /// </summary>
        /// <param name="services">Services.</param>
        /// <param name="setupAction">Setup action.</param>
        public static IServiceCollection AddBloomFilter(this IServiceCollection services, Action<BloomFilterOptions> setupAction)
        {
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            //Options and extension service
            var options = new BloomFilterOptions();
            setupAction(options);
            foreach (var serviceExtension in options.Extensions)
            {
                serviceExtension.AddServices(services);
            }
            services.AddSingleton(options);

            return services;
        }

        /// <summary>
        /// Uses the in-memory.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="name"></param>
        /// <param name="setupActions"></param>
        public static BloomFilterOptions UseInMemory(this BloomFilterOptions options,
            string name = BloomFilterConstValue.DefaultInMemoryName, Action<FilterMemoryOptions>? setupActions = null)
        {
            var filterMemoryOptions = new FilterMemoryOptions
            {
                Name = name
            };
            setupActions?.Invoke(filterMemoryOptions);
            options.RegisterExtension(new FilterMemoryOptionsExtension(filterMemoryOptions));
            return options;
        }

        /// <summary>
        /// Uses the in-memory.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="filterMemoryOptions"></param>
        public static BloomFilterOptions UseInMemory(this BloomFilterOptions options, FilterMemoryOptions filterMemoryOptions)
        {
            if (filterMemoryOptions == null) throw new ArgumentNullException(nameof(filterMemoryOptions));
            options.RegisterExtension(new FilterMemoryOptionsExtension(filterMemoryOptions));
            return options;
        }
    }
}