using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloomFilter
{
    /// <summary>
    /// Represents a Bloom filter and provides
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Filter" />
    public abstract class Filter<T> : Filter
    {

        private bool isBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Filter{T}"/> class.
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// expectedElements - expectedElements must be > 0
        /// or
        /// errorRate
        /// </exception>
        /// <exception cref="System.ArgumentNullException">hashFunction</exception>
        public Filter(int expectedElements, double errorRate, HashFunction hashFunction)
            : base(expectedElements, errorRate, hashFunction)
        {
            CheckElementType();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Filter{T}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="hashes">The hashes.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// capacity - capacity must be > 0
        /// or
        /// hashes - hashes must be > 0
        /// </exception>
        /// <exception cref="System.ArgumentNullException">hashFunction</exception>
        public Filter(int capacity, int hashes, HashFunction hashFunction) : base(capacity, hashes, hashFunction)
        {
            CheckElementType();
        }


        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public bool Add(T element)
        {
            return Add(ToBytes(element));
        }

        /// <summary>
        /// Async Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public Task<bool> AddAsync(T element)
        {
            return AddAsync(ToBytes(element));
        }

        /// <summary>
        /// Adds the specified elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public virtual IList<bool> Add(IEnumerable<T> elements)
        {
            return elements.Select(e => Add(e)).ToList();
        }

        /// <summary>
        /// Async Adds the specified elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public async virtual Task<IList<bool>> AddAsync(IEnumerable<T> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await AddAsync(el));
            }
            return result;
        }

        /// <summary>
        /// Tests whether an element is present in the filter
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool Contains(T element)
        {
            return Contains(ToBytes(element));
        }

        /// <summary>
        /// Async Tests whether an element is present in the filter
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Task<bool> ContainsAsync(T element)
        {
            return ContainsAsync(ToBytes(element));
        }

        /// <summary>
        /// Tests whether an elements is present in the filter
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public virtual IList<bool> Contains(IEnumerable<T> elements)
        {
            return elements.Select(e => Contains(e)).ToList();
        }

        /// <summary>
        /// Async Tests whether an elements is present in the filter
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public async virtual Task<IList<bool>> ContainsAsync(IEnumerable<T> elements)
        {
            var result = new List<bool>();
            foreach (var el in elements)
            {
                result.Add(await ContainsAsync(el));
            }
            return result;
        }

        /// <summary>
        /// Alls the specified elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public bool All(IEnumerable<T> elements)
        {
            return Contains(elements).All(e => e);
        }

        /// <summary>
        /// Async Alls the specified elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public async Task<bool> AllAsync(IEnumerable<T> elements)
        {
            return (await ContainsAsync(elements)).All(e => e);
        }

        /// <summary>
        /// Converts the element to UTF8 bytes
        /// </summary>
        /// <param name="elemnt">The elemnt.</param>
        /// <returns></returns>
        protected virtual byte[] ToBytes(T elemnt)
        {
            if (isBytes) return elemnt as byte[];

            return Encoding.UTF8.GetBytes(Convert.ToString(elemnt, CultureInfo.InvariantCulture));
        }


        private void CheckElementType()
        {
            var type = typeof(T);

            if (type == typeof(byte[]))
            {
                isBytes = true;
                return;
            }

            var typeCode = Type.GetTypeCode(Nullable.GetUnderlyingType(type) ?? type);

            switch (typeCode)
            {
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
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

    }
}