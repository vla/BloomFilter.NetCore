using System;
using System.Threading.Tasks;

namespace BloomFilter.Redis;

/// <summary>
/// Redis BitMap operate Interface
/// </summary>
public interface IRedisBitOperate : IDisposable
{
    bool[] Set(string redisKey, uint[] positions, bool value);

    Task<bool[]> SetAsync(string redisKey, uint[] positions, bool value);

    bool Set(string redisKey, uint position, bool value);

    Task<bool> SetAsync(string redisKey, uint position, bool value);

    bool Get(string redisKey, uint position);

    Task<bool> GetAsync(string redisKey, uint position);

    bool[] Get(string redisKey, uint[] positions);

    Task<bool[]> GetAsync(string redisKey, uint[] positions);

    void Clear(string redisKey);

    Task ClearAsync(string redisKey);
}