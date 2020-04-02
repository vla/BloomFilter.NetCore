using System.Collections;
using System.Security.Cryptography;

namespace BloomFilter.HashAlgorithms
{

    public class HashCryptoSHA1 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm = SHA1.Create();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }

    public class HashCryptoSHA256 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm = SHA256.Create();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }

    public class HashCryptoSHA384 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm = SHA384.Create();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }

    public class HashCryptoSHA512 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm = SHA512.Create();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }

    public abstract class HashCrypto : HashFunction
    {
        protected int[] ComputeHash(HashAlgorithm hashAlgorithm, byte[] data, int m, int k)
        {
            int[] positions = new int[k];

            int computedHashes = 0;

            byte[] digest = new byte[0];
            byte[] output = new byte[hashAlgorithm.HashSize / 8];

            while (computedHashes < k)
            {
                hashAlgorithm.TransformBlock(digest, 0, digest.Length, output, 0);
                digest = hashAlgorithm.ComputeHash(data, 0, data.Length);

                BitArray hashed = new BitArray(digest);

                int filterSize = 32 - (int)NumberOfLeadingZeros((uint)m);

                int hashBits = digest.Length * 8;

                for (int split = 0; split < (hashBits / filterSize) && computedHashes < k; split++)
                {
                    int from = split * filterSize;
                    int to = ((split + 1) * filterSize);

                    int intHash = BitToIntOne(hashed, from, to);

                    if (intHash < m)
                    {
                        positions[computedHashes] = intHash;
                        computedHashes++;
                    }
                }
            }

            return positions;
        }
    }
}