using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BloomFilter.Configurations;

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
            if (_options.Bits is not null)
            {
                return new FilterMemory(
                    _options.Bits,
                    _options.BitsMore,
                    _options.Name,
                    _options.ExpectedElements,
                    _options.ErrorRate,
                    HashFunction.Functions[_options.Method]);
            }

            if (_options.Bytes is not null)
            {
                return new FilterMemory(
                    _options.Bytes,
                    _options.BytesMore,
                    _options.Name,
                    _options.ExpectedElements,
                    _options.ErrorRate,
                    HashFunction.Functions[_options.Method]);
            }

            return new FilterMemory(
                _options.Name,
                _options.ExpectedElements,
                _options.ErrorRate,
                HashFunction.Functions[_options.Method]);
        });
    }
}