using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloomFilter
{
    /// <summary>
    /// Represents a Bloom filter and provides
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IBloomFilter" />
    public abstract class Filter<T> : IBloomFilter
    {
        public HashFunction Hash { get; }

        /// <summary>
        /// the Capacity of the Bloom filter
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// number of hash functions
        /// </summary>
        public int Hashes { get; }

        /// <summary>
        ///  the expected elements.
        /// </summary>
        public int ExpectedElements { get; }

        /// <summary>
        /// the number of expected elements
        /// </summary>
        public double ErrorRate { get; }

        public Filter(int expectedElements, double errorRate, HashFunction hashFunction)
        {
            if (expectedElements < 1)
                throw new ArgumentOutOfRangeException("expectedElements", expectedElements, "expectedElements must be > 0");
            if (errorRate >= 1 || errorRate <= 0)
                throw new ArgumentOutOfRangeException("errorRate", errorRate, string.Format("errorRate must be between 0 and 1, exclusive. Was {0}", errorRate));

            ExpectedElements = expectedElements;
            ErrorRate = errorRate;
            Hash = hashFunction ?? throw new ArgumentNullException(nameof(hashFunction));

            Capacity = BestM(expectedElements, errorRate);
            Hashes = BestK(expectedElements, Capacity);

            CheckElementType();
        }

        public Filter(int capacity, int hashes, HashFunction hashFunction)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException("capacity", capacity, "capacity must be > 0");
            if (capacity < 1)
                throw new ArgumentOutOfRangeException("hashes", hashes, "hashes must be > 0");

            Capacity = capacity;
            Hashes = hashes;
            Hash = hashFunction ?? throw new ArgumentNullException(nameof(hashFunction));

            ExpectedElements = BestN(hashes, capacity);
            ErrorRate = BestP(hashes, capacity, ExpectedElements);

            CheckElementType();
        }

        public abstract bool Add(byte[] element);

        public bool Add(T element)
        {
            return Add(ToBytes(element));
        }

        public virtual IList<bool> Add(IEnumerable<T> elements)
        {
            return elements.Select(e => Add(e)).ToList();
        }

        public abstract void Clear();

        public abstract bool Contains(byte[] element);

        public bool Contains(T element)
        {
            return Contains(ToBytes(element));
        }

        public virtual IList<bool> Contains(IEnumerable<T> elements)
        {
            return elements.Select(e => Contains(e)).ToList();
        }

        public virtual bool All(IEnumerable<T> elements)
        {
            return elements.Select(e => Contains(e)).All(e => e);
        }

        public int[] ComputeHash(byte[] data)
        {
            return Hash.ComputeHash(data, Capacity, Hashes);
        }


        /// <summary>
        /// Converts the element to UTF8 bytes
        /// </summary>
        /// <param name="elemnt">The elemnt.</param>
        /// <returns></returns>
        protected virtual byte[] ToBytes(T elemnt)
        {
            return Encoding.UTF8.GetBytes(elemnt.ToString());
        }

        /// <summary>
        /// Calculates the optimal size of the bloom filter in bits given expectedElements (expected
        /// number of elements in bloom filter) and falsePositiveProbability (tolerable false positive rate).
        /// </summary>
        /// <param name="n">Expected number of elements inserted in the bloom filter</param>
        /// <param name="p">Tolerable false positive rate</param>
        /// <returns>the optimal siz of the bloom filter in bits</returns>
        public static int BestM(long n, double p)
        {
            return (int)Math.Ceiling(-1 * (n * Math.Log(p)) / Math.Pow(Math.Log(2), 2));
        }

        /// <summary>
        /// Calculates the optimal hashes (number of hash function) given expectedElements (expected number of
        /// elements in bloom filter) and size (size of bloom filter in bits).
        /// </summary>
        /// <param name="n">Expected number of elements inserted in the bloom filter</param>
        /// <param name="m">The size of the bloom filter in bits.</param>
        /// <returns>the optimal amount of hash functions hashes</returns>
        public static int BestK(long n, long m)
        {
            return (int)Math.Ceiling((Math.Log(2) * m) / n);
        }

        /// <summary>
        /// Calculates the amount of elements a Bloom filter for which the given configuration of size and hashes is optimal.
        /// </summary>
        /// <param name="k">number of hashes</param>
        /// <param name="m">The size of the bloom filter in bits.</param>
        /// <returns>mount of elements a Bloom filter for which the given configuration of size and hashes is optimal</returns>
        public static int BestN(long k, long m)
        {
            return (int)Math.Ceiling((Math.Log(2) * m) / k);
        }

        /// <summary>
        /// Calculates the best-case (uniform hash function) false positive probability.
        /// </summary>
        /// <param name="k"> number of hashes</param>
        /// <param name="m">The size of the bloom filter in bits.</param>
        /// <param name="insertedElements">number of elements inserted in the filter</param>
        /// <returns>The calculated false positive probability</returns>
        public static double BestP(long k, long m, double insertedElements)
        {
            return Math.Pow((1 - Math.Exp(-k * insertedElements / (double)m)), k);
        }

        private void CheckElementType()
        {
            var type = typeof(T);
            var typeCode = Type.GetTypeCode(Nullable.GetUnderlyingType(type) ?? type);

            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.String:
                case TypeCode.DateTime:
                    //OK
                    break;

                default:
                    throw new NotSupportedException("Element type are not supported " + type.Name);
            }
        }

        public override string ToString()
        {
            return $"Capacity:{Capacity},Hashes:{Hashes},ExpectedElements:{ExpectedElements},ErrorRate:{ErrorRate}";
        }
    }
}