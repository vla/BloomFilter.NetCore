using System.Runtime.CompilerServices;

namespace BloomFilter.HashAlgorithms
{
    public class XXHash32 : HashFunction
    {
        private static readonly uint Prime32_1 = 0x9E3779B1U;
        private static readonly uint Prime32_2 = 0x85EBCA77U;
        private static readonly uint Prime32_3 = 0xC2B2AE3DU;
        private static readonly uint Prime32_4 = 0x27D4EB2FU;
        private static readonly uint Prime32_5 = 0x165667B1U;

        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            int[] positions = new int[k];
            var hash1 = UnsafeComputeHash(data, data.Length, 0);
            var hash2 = UnsafeComputeHash(data, data.Length, hash1);
            for (int i = 0; i < k; i++)
            {
                positions[i] = (int)((hash1 + i * hash2) % m);
            }
            return positions;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe uint UnsafeComputeHash(byte[] data, int length, uint seed = 0)
        {
            fixed (byte* pData = &data[0])
            {
                return UnsafeComputeHash(pData, length, seed);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe uint UnsafeComputeHash(byte* input, int len, uint seed)
        {
            uint h32;

            if (len >= 16)
            {
                byte* end = input + len;
                byte* limit = end - 15;

                uint v1 = seed + Prime32_1 + Prime32_2;
                uint v2 = seed + Prime32_2;
                uint v3 = seed + 0;
                uint v4 = seed - Prime32_1;

                do
                {
                    var reg1 = *((uint*)(input + 0));
                    var reg2 = *((uint*)(input + 4));
                    var reg3 = *((uint*)(input + 8));
                    var reg4 = *((uint*)(input + 12));

                    // XXH32_round
                    v1 += reg1 * Prime32_2;
                    v1 = (v1 << 13) | (v1 >> (32 - 13));
                    v1 *= Prime32_1;

                    // XXH32_round
                    v2 += reg2 * Prime32_2;
                    v2 = (v2 << 13) | (v2 >> (32 - 13));
                    v2 *= Prime32_1;

                    // XXH32_round
                    v3 += reg3 * Prime32_2;
                    v3 = (v3 << 13) | (v3 >> (32 - 13));
                    v3 *= Prime32_1;

                    // XXH32_round
                    v4 += reg4 * Prime32_2;
                    v4 = (v4 << 13) | (v4 >> (32 - 13));
                    v4 *= Prime32_1;

                    input += 16;
                } while (input < limit);

                h32 = ((v1 << 1) | (v1 >> (32 - 1))) +
                      ((v2 << 7) | (v2 >> (32 - 7))) +
                      ((v3 << 12) | (v3 >> (32 - 12))) +
                      ((v4 << 18) | (v4 >> (32 - 18)));
            }
            else
            {
                h32 = seed + Prime32_5;
            }

            h32 += (uint)len;

            // XXH32_finalize
            len &= 15;
            while (len >= 4)
            {
                h32 += *((uint*)input) * Prime32_3;
                input += 4;
                h32 = ((h32 << 17) | (h32 >> (32 - 17))) * Prime32_4;
                len -= 4;
            }

            while (len > 0)
            {
                h32 += *((byte*)input) * Prime32_5;
                ++input;
                h32 = ((h32 << 11) | (h32 >> (32 - 11))) * Prime32_1;
                --len;
            }

            // XXH32_avalanche
            h32 ^= h32 >> 15;
            h32 *= Prime32_2;
            h32 ^= h32 >> 13;
            h32 *= Prime32_3;
            h32 ^= h32 >> 16;

            return h32;
        }
    }
}