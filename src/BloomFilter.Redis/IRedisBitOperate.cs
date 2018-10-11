namespace BloomFilter.Redis
{
    /// <summary>
    /// Redis BitMap operate Interface
    /// </summary>
    public interface IRedisBitOperate
    {
        bool[] Set(string redisKey, int[] positions, bool value);

        bool Set(string redisKey, int position, bool value);

        bool Get(string redisKey, int position);

        bool[] Get(string redisKey, int[] positions);

        void Clear(string redisKey);
    }
}