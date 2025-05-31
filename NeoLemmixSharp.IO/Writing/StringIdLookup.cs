using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoLemmixSharp.IO.Writing;

internal readonly struct StringIdLookup
{
    private const int MaxStackByteBufferSize = 256;

    private readonly Dictionary<string, ushort> _lookup;
    internal int Count => _lookup.Count;

    public StringIdLookup()
    {
        _lookup = new Dictionary<string, ushort>(ReadWriteHelpers.InitialStringListCapacity);
    }

    internal void RecordString(string? s)
    {
        if (string.IsNullOrEmpty(s))
            return;

        // IDs start at 1, ID 0 is associated with the empty string
        // We don't serialise the empty string though
        var nextStringId = (ushort)(1 + _lookup.Count);

        ref var correspondingStringId = ref CollectionsMarshal.GetValueRefOrAddDefault(_lookup, s, out var exists);
        if (!exists)
        {
            correspondingStringId = nextStringId;
        }
    }

    [Pure]
    internal ushort GetStringId(string? s)
    {
        if (string.IsNullOrEmpty(s))
            return 0;

        return _lookup[s];
    }

    [Pure]
    private int CalculateBufferSize()
    {
        var maxBufferSize = 0;

        foreach (var kvp in _lookup)
        {
            maxBufferSize = Math.Max(maxBufferSize, Encoding.UTF8.GetByteCount(kvp.Key));
        }

        return maxBufferSize;
    }

    [SkipLocalsInit]
    internal void WriteStrings<TPerfectHasher, TEnum>(RawFileDataWriter<TPerfectHasher, TEnum> writer)
        where TPerfectHasher : struct, ISectionIdentifierHelper<TEnum>
        where TEnum : unmanaged, Enum
    {
        var bufferSize = CalculateBufferSize();

        FileWritingException.WriterAssert(bufferSize <= ushort.MaxValue, "Cannot serialize a string larger than 65535 bytes!");

        Span<byte> buffer = bufferSize > MaxStackByteBufferSize
            ? new byte[bufferSize]
            : stackalloc byte[bufferSize];

        foreach (var kvp in _lookup.OrderBy(x => x.Value))
        {
            var stringToWrite = kvp.Key;
            var id = kvp.Value;

            writer.Write(id);

            var byteCount = Encoding.UTF8.GetBytes(stringToWrite, buffer);

            writer.Write((ushort)byteCount);
            writer.Write(buffer[..byteCount]);
        }
    }
}
