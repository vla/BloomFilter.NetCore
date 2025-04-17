using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BloomFilter.Configurations;

public class FilterMemoryOptionsExtension : IBloomFilterOptionsExtension
{
    private readonly FilterMemoryOptions _options;
    private readonly Type? _serializerType;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterMemoryOptionsExtension"/> class.
    /// </summary>
    /// <param name="options">Configure.</param>
    /// <param name="serializerType"></param>
    public FilterMemoryOptionsExtension(FilterMemoryOptions options, Type? serializerType = null)
    {
        _options = options;
        _serializerType = serializerType;
    }

    public void AddServices(IServiceCollection services)
    {
        services.TryAddSingleton<IBloomFilterFactory, DefaultBloomFilterFactory>();

        if (_serializerType is null)
        {
            services.TryAddSingleton<IFilterMemorySerializer, DefaultFilterMemorySerializer>();
        }
        else
        {
            services.TryAddSingleton(typeof(IFilterMemorySerializer), _serializerType);
        }

        services.AddSingleton<IBloomFilter, FilterMemory>(x =>
        {
            return new FilterMemory(_options, x.GetRequiredService<IFilterMemorySerializer>());
        });
    }
}