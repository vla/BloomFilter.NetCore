using System.Runtime.CompilerServices;

namespace BloomFilter.HashAlgorithms
{
    public class XXHash64 : HashFunction
    {
        private static readonly ulong Prime64_1 = 0x9E3779B185EBCA87UL;
        private static readonly ulong Prime64_2 = 0xC2B2AE3D27D4EB4FUL;
        private static readonly ulong Prime64_3 = 0x165667B19E3779F9UL;
        private static readonly ulong Prime64_4 = 0x85EBCA77C2B2AE63UL;
        private static readonly ulong Prime64_5 = 0x27D4EB2F165667C5UL;

        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            int[] positions = new int[k];

            var hash = UnsafeComputeHash(data, data.Length, 0);

            uint hash1 = (uint)(hash & uint.MaxValue);
            uint hash2 = (uint)(hash >> 32);

            for (int i = 0; i < k; i++)
            {
                positions[i] = (int)((hash1 + i * hash2) % m);
            }
            return positions;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong UnsafeComputeHash(byte[] data, int length, ulong seed = 0)
        {
            fixed (byte* pData = &data[0])
            {
                return UnsafeComputeHash(pData, length, seed);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong UnsafeComputeHash(byte* input, int len, ulong seed)
        {
            ulong h64;

            if (len >= 32)
            {
                byte* end = input + len;
                byte* limit = end - 31;

                ulong v1 = seed + Prime64_1 + Prime64_2;
                ulong v2 = seed + Prime64_2;
                ulong v3 = seed + 0;
                ulong v4 = seed - Prime64_1;

                do
                {
                    var reg1 = *((ulong*)(input + 0));
                    var reg2 = *((ulong*)(input + 8));
                    var reg3 = *((ulong*)(input + 16));
                    var reg4 = *((ulong*)(input + 24));

                    // XXH64_round
                    v1 += reg1 * Prime64_2;
                    v1 = (v1 << 31) | (v1 >> (64 - 31));
                    v1 *= Prime64_1;

                    // XXH64_round
                    v2 += reg2 * Prime64_2;
                    v2 = (v2 << 31) | (v2 >> (64 - 31));
                    v2 *= Prime64_1;

                    // XXH64_round
                    v3 += reg3 * Prime64_2;
                    v3 = (v3 << 31) | (v3 >> (64 - 31));
                    v3 *= Prime64_1;

                    // XXH64_round
                    v4 += reg4 * Prime64_2;
                    v4 = (v4 << 31) | (v4 >> (64 - 31));
                    v4 *= Prime64_1;
                    input += 32;
                } while (input < limit);

                h64 = ((v1 << 1) | (v1 >> (64 - 1))) +
                      ((v2 << 7) | (v2 >> (64 - 7))) +
                      ((v3 << 12) | (v3 >> (64 - 12))) +
                      ((v4 << 18) | (v4 >> (64 - 18)));

                // XXH64_mergeRound
                v1 *= Prime64_2;
                v1 = (v1 << 31) | (v1 >> (64 - 31));
                v1 *= Prime64_1;
                h64 ^= v1;
                h64 = h64 * Prime64_1 + Prime64_4;

                // XXH64_mergeRound
                v2 *= Prime64_2;
                v2 = (v2 << 31) | (v2 >> (64 - 31));
                v2 *= Prime64_1;
                h64 ^= v2;
                h64 = h64 * Prime64_1 + Prime64_4;

                // XXH64_mergeRound
                v3 *= Prime64_2;
                v3 = (v3 << 31) | (v3 >> (64 - 31));
                v3 *= Prime64_1;
                h64 ^= v3;
                h64 = h64 * Prime64_1 + Prime64_4;

                // XXH64_mergeRound
                v4 *= Prime64_2;
                v4 = (v4 << 31) | (v4 >> (64 - 31));
                v4 *= Prime64_1;
                h64 ^= v4;
                h64 = h64 * Prime64_1 + Prime64_4;
            }
            else
            {
                h64 = seed + Prime64_5;
            }

            h64 += (ulong)len;

            // XXH64_finalize
            len &= 31;
            while (len >= 8)
            {
                ulong k1 = XXH64_round(0, *(ulong*)input);
                input += 8;
                h64 ^= k1;
                h64 = XXH_rotl64(h64, 27) * Prime64_1 + Prime64_4;
                len -= 8;
            }
            if (len >= 4)
            {
                h64 ^= *(uint*)input * Prime64_1;
                input += 4;
                h64 = XXH_rotl64(h64, 23) * Prime64_2 + Prime64_3;
                len -= 4;
            }
            while (len > 0)
            {
                h64 ^= (*input++) * Prime64_5;
                h64 = XXH_rotl64(h64, 11) * Prime64_1;
                --len;
            }

            // XXH64_avalanche
            h64 ^= h64 >> 33;
            h64 *= Prime64_2;
            h64 ^= h64 >> 29;
            h64 *= Prime64_3;
            h64 ^= h64 >> 32;

            return h64;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong XXH64_round(ulong acc, ulong input)
        {
            acc += input * Prime64_2;
            acc = XXH_rotl64(acc, 31);
            acc *= Prime64_1;
            return acc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong XXH_rotl64(ulong x, int r)
        {
            return (x << r) | (x >> (64 - r));
        }
    }
}