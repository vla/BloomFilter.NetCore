namespace BloomFilter
{
    /// <summary>
    /// Hash Methods
    /// </summary>
    public enum HashMethod
    {
        LCGWithFNV1,
        LCGWithFNV1a,
        LCGModifiedFNV1,
        RNGWithFNV1,
        RNGWithFNV1a,
        RNGModifiedFNV1,
        CRC32,
        CRC32u,
        Adler32,
        Murmur2,
        Murmur3,
        Murmur3KirschMitzenmacher,
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }
}