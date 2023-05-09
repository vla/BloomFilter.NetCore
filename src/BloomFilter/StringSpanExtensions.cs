using System;
using System.Text;

namespace BloomFilter;

internal static class StringSpanExtensions
{
    public static ReadOnlyMemory<byte> ToUtf8(this ReadOnlySpan<char> value)
    {
#if NET6_0_OR_GREATER
        Memory<byte> bytes = new byte[Encoding.UTF8.GetByteCount(value)];
        var bytesWritten = Encoding.UTF8.GetBytes(value, bytes.Span);
        return bytes.Slice(0, bytesWritten);

#else
        var chars = value.ToArray();
        var bytes = new byte[Encoding.UTF8.GetByteCount(chars)];
        var bytesWritten = Encoding.UTF8.GetBytes(chars, 0, value.Length, bytes, 0);
        return new ReadOnlyMemory<byte>(bytes, 0, bytesWritten);
#endif
    }

    public static ReadOnlyMemory<char> FromUtf8(this ReadOnlySpan<byte> source)
    {
#if NET6_0_OR_GREATER
        source = source.WithoutBom();
        Memory<char> chars = new char[Encoding.UTF8.GetCharCount(source)];
        var charsWritten = Encoding.UTF8.GetChars(source, chars.Span);
        return chars.Slice(0, charsWritten);
#else
        var bytes = source.WithoutBom().ToArray();
        var chars = new char[Encoding.UTF8.GetCharCount(bytes)];
        var charsWritten = Encoding.UTF8.GetChars(bytes, 0, bytes.Length, chars, 0);
        return new ReadOnlyMemory<char>(chars, 0, charsWritten);

#endif
    }

    public static byte[] ToUtf8Bytes(this ReadOnlySpan<char> value)
    {
        return ToUtf8(value).ToArray();
    }

    public static ReadOnlySpan<char> WithoutBom(this ReadOnlySpan<char> value)
    {
        return value.Length > 0 && value[0] == 65279
            ? value.Slice(1)
            : value;
    }

    public static ReadOnlySpan<byte> WithoutBom(this ReadOnlySpan<byte> value)
    {
        return value.Length > 3 && value[0] == 0xEF && value[1] == 0xBB && value[2] == 0xBF
            ? value.Slice(3)
            : value;
    }
}