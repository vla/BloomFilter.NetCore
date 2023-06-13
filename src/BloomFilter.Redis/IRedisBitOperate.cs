using System;
using System.Threading.Tasks;

namespace BloomFilter.Redis;

/// <summary>
/// Redis BitMap operate Interface
/// </summary>
public interface IRedisBitOperate : IDisposable
{
    bool[] Set(string redisKey, long[] positions, bool value);

    Task<bool[]> SetAsync(string redisKey, long[] positions, bool value);

    bool Set(string redisKey, long position, bool value);

    Task<bool> SetAsync(string redisKey, long position, bool value);

    bool Get(string redisKey, long position);

    Task<bool> GetAsync(string redisKey, long position);

    bool[] Get(string redisKey, long[] positions);

    Task<bool[]> GetAsync(string redisKey, long[] positions);

    void Clear(string redisKey);

    Task ClearAsync(string redisKey);
}