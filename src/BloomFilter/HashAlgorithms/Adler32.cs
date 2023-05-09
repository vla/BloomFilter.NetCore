using System;

namespace BloomFilter.HashAlgorithms;

public class Adler32 : HashFunction
{
    private const int seed32 = 89478583;

    public override uint[] ComputeHash(ReadOnlySpan<byte> data, uint m, uint k)
    {
        uint[] positions = new uint[k];
        int hashes = 0;
        int salt = 0;

        var adler32 = new Internal.Adler32();

        while (hashes < k)
        {
            adler32.Append(data);
            adler32.Append(BitConverter.GetBytes(hashes + salt++ + seed32));

            var adlerValue = adler32.GetCurrentHashAsUInt32();
            adler32.Reset();

            long hash = BinaryHelper.Rejection(adlerValue, m);

            if (hash != -1)
            {
                positions[hashes++] = (uint)hash;
            }
        }
        return positions;
    }
}