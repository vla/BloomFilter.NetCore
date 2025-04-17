using System.IO;
using System.Threading.Tasks;

namespace BloomFilter;

/// <summary>
///  Represents a <see cref="FilterMemory"/> serialization interface
/// </summary>
public interface IFilterMemorySerializer
{
    /// <summary>
    /// Serialize to a stream
    /// </summary>
    /// <param name="param"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    ValueTask SerializeAsync(FilterMemorySerializerParam param, Stream stream);

    /// <summary>
    /// Deserialize from the stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    ValueTask<FilterMemorySerializerParam> DeserializeAsync(Stream stream);
}