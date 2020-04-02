using BloomFilter.HashAlgorithms.Checksum;
using System;

namespace BloomFilter.HashAlgorithms
{
    public class HashChecksumCrc32 : HashChecksum
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var checkSum = new Crc32();
            return ComputeHash(checkSum, data, m, k);
        }
    }

    public class HashChecksumCrc32u : HashChecksum
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var checkSum = new Crc32u();
            return ComputeHash(checkSum, data, m, k);
        }
    }

    public class HashChecksumAdler32 : HashChecksum
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var checkSum = new Adler32();
            return ComputeHash(checkSum, data, m, k);
        }
    }

    public abstract class HashChecksum : HashFunction
    {
        protected int[] ComputeHash(IChecksum checksum, byte[] data, int m, int k)
        {
            if (checksum == null)
            {
                throw new ArgumentNullException(nameof(checksum));
            }

            int[] positions = new int[k];
            int hashes = 0;
            int salt = 0;

            const int seed32 = 89478583;

            while (hashes < k)
            {
                checksum.Reset();
                checksum.Update(data);
                checksum.Update(hashes + salt++ + seed32);
                int hash = Rejection((int)checksum.Value, m);
                if (hash != -1)
                {
                    positions[hashes++] = hash;
                }
            }
            return positions;
        }
    }
}