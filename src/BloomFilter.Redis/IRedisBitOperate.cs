using System;
using System.Threading.Tasks;

namespace BloomFilter.Redis
{
    /// <summary>
    /// Redis BitMap operate Interface
    /// </summary>
    public interface IRedisBitOperate : IDisposable
    {
        bool[] Set(string redisKey, int[] positions, bool value);

        Task<bool[]> SetAsync(string redisKey, int[] positions, bool value);

        bool Set(string redisKey, int position, bool value);

        Task<bool> SetAsync(string redisKey, int position, bool value);

        bool Get(string redisKey, int position);

        Task<bool> GetAsync(string redisKey, int position);

        bool[] Get(string redisKey, int[] positions);

        Task<bool[]> GetAsync(string redisKey, int[] positions);

        void Clear(string redisKey);

        Task ClearAsync(string redisKey);
    }
}