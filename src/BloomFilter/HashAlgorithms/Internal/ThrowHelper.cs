using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace BloomFilter.HashAlgorithms.Internal;

internal static partial class ThrowHelper
{
#if NET7_0_OR_GREATER
    [DoesNotReturn]

    internal static void ThrowUnreachableException() => throw new UnreachableException();
#endif
}