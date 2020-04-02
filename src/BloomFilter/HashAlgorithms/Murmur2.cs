namespace BloomFilter.HashAlgorithms
{
    public class Murmur2 : HashFunction
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            int[] positions = new int[k];

            const uint seed = 89478583;

            int hashes = 0;
            while (hashes < k)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == 0xff)
                    {
                        data[i] = 0;
                        continue;
                    }
                    else
                    {
                        data[i]++;
                        break;
                    }
                }

                uint hash = MurmurHash2(seed, data, 0, data.Length);

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
            const uint M = 0x5bd1e995;
            const int R = 24;

            uint seed = (uint)(0xdeadbeef * length);
            uint hash = (uint)(seed32 ^ length);

            int count = length >> 2;

            fixed (byte* start = &(data[offset]))
            {
                uint* ptrUInt = (uint*)start;

                while (count > 0)
                {
                    uint current = *ptrUInt;

                    current *= M;
                    current ^= current >> R;
                    current *= M;
                    hash *= M;
                    hash ^= current;

                    count--;
                    ptrUInt++;
                }

                switch (length & 3)
                {
                    case 3:
                        // reverse the last 3 bytes and convert it to an uint
                        // so cast the last to into an UInt16 and get the 3rd as a byte
                        // ABC --> CBA; (UInt16)(AB) --> BA
                        //h ^= (uint)(*ptrByte);
                        //h ^= (uint)(ptrByte[1] << 8);
                        hash ^= (*(ushort*)ptrUInt);
                        hash ^= (uint)(((byte*)ptrUInt)[2] << 16);
                        hash *= M;
                        break;

                    case 2:
                        hash ^= (*(ushort*)ptrUInt);
                        hash *= M;
                        break;

                    case 1:
                        hash ^= (*((byte*)ptrUInt));
                        hash *= M;
                        break;
                }
            }

            hash ^= hash >> 13;
            hash *= M;
            hash ^= hash >> 15;

            return hash;
        }
    }
}