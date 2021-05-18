namespace BloomFilter.HashAlgorithms
{
    public class Murmur2 : HashFunction
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            int[] positions = new int[k];

            const uint seed = 89478583;

            var value = (byte[])data.Clone();

            int hashes = 0;
            while (hashes < k)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (value[i] == 0xff)
                    {
                        value[i] = 0;
                        continue;
                    }
                    else
                    {
                        value[i]++;
                        break;
                    }
                }

                uint hash = MurmurHash2(seed, value, 0, value.Length);

                int lastHash = Rejection(hash, m);
                if (lastHash != -1)
                {
                    positions[hashes++] = lastHash;
                }
            }
            return positions;
        }

        // this could be rewritten to support streams tho
        // cache the tail of the buffer if its length is not mod4 then merge with the next buffer (this is a perf hit since we cannot do our pointer magics)
        // then the swicth and the last XORs could be moved into TransformFinal
        // -- or --
        // just cache tail and if we have a cache dvalue and the next block is not mod4 long then throw an exception (thus only allow random length blocks for the last one)
        private static unsafe uint MurmurHash2(uint seed32, byte[] data, int offset, int length)
        {
            const int M = 0x5bd1e995;
            const int R = 24;

            int len = data.Length;
            long hash = (uint)(seed32 ^ length);

            int i = 0;
            while (len >= 4)
            {
                int k = data[i + 0] & 0xFF;
                k |= (data[i + 1] & 0xFF) << 8;
                k |= (data[i + 2] & 0xFF) << 16;
                k |= (data[i + 3] & 0xFF) << 24;

                k *= M;
                k ^= RightMove(k, R);
                k *= M;

                hash *= M;
                hash ^= k;

                i += 4;
                len -= 4;
            }

            switch (len)
            {
                // reverse the last 3 bytes and convert it to an uint
                // so cast the last to into an UInt16 and get the 3rd as a byte
                // ABC --> CBA; (UInt16)(AB) --> BA
                //h ^= (uint)(*ptrByte);
                //h ^= (uint)(ptrByte[1] << 8);
                case 3:
                    hash ^= (data[i + 2] & 0xFF) << 16;
                    hash ^= (data[i + 1] & 0xFF) << 8;
                    hash ^= (data[i + 0] & 0xFF);
                    hash *= M;
                    break;

                case 2:
                    hash ^= (data[i + 1] & 0xFF) << 8;
                    hash ^= (data[i + 0] & 0xFF);
                    hash *= M;
                    break;

                case 1:
                    hash ^= (data[i + 0] & 0xFF);
                    hash *= M;
                    break;
            }

            hash ^= RightMove(hash, 13);
            hash *= M;
            hash ^= RightMove(hash, 15);

            return (uint)hash;
        }
    }
}