using System;
using BloomFilter;
using BloomFilter.Configurations;

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
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(setupAction);
#else
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));
#endif

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
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(filterMemoryOptions);
#else
            if (filterMemoryOptions == null) throw new ArgumentNullException(nameof(filterMemoryOptions));
#endif
            options.RegisterExtension(new FilterMemoryOptionsExtension(filterMemoryOptions));
            return options;
        }

        /// <summary>
        /// Uses the in-memory.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="name"></param>
        /// <param name="setupActions"></param>
        public static BloomFilterOptions UseInMemoryWithSerializer<TSerializer>(this BloomFilterOptions options,
            string name = BloomFilterConstValue.DefaultInMemoryName, Action<FilterMemoryOptions>? setupActions = null)
            where TSerializer : IFilterMemorySerializer
        {
            var filterMemoryOptions = new FilterMemoryOptions
            {
                Name = name
            };
            setupActions?.Invoke(filterMemoryOptions);
            options.RegisterExtension(new FilterMemoryOptionsExtension(filterMemoryOptions, typeof(TSerializer)));
            return options;
        }

        /// <summary>
        /// Uses the in-memory.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="filterMemoryOptions"></param>
        public static BloomFilterOptions UseInMemoryWithSerializer<TSerializer>(this BloomFilterOptions options, FilterMemoryOptions filterMemoryOptions)
              where TSerializer : IFilterMemorySerializer
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(filterMemoryOptions);
#else
            if (filterMemoryOptions == null) throw new ArgumentNullException(nameof(filterMemoryOptions));
#endif
            options.RegisterExtension(new FilterMemoryOptionsExtension(filterMemoryOptions, typeof(TSerializer)));
            return options;
        }
    }
}