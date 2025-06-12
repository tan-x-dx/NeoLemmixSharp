using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoLemmixSharp.IO.Writing;

internal readonly struct StringIdLookup
{
    private readonly Dictionary<string, ushort> _lookup;

    internal StringIdLookup(Dictionary<string, ushort> lookup) => _lookup = lookup;

    [Pure]
    internal ushort GetStringId(StyleIdentifier styleIdentifier) => GetStringId(styleIdentifier.ToString());
    [Pure]
    internal ushort GetStringId(PieceIdentifier pieceIdentifier) => GetStringId(pieceIdentifier.ToString());
    [Pure]
    internal ushort GetStringId(GadgetInputName gadgetInputName) => GetStringId(gadgetInputName.ToString());

    [Pure]
    internal ushort GetStringId(string? s)
    {
        if (string.IsNullOrEmpty(s))
            return 0;

        return _lookup[s];
    }
}

internal readonly struct MutableStringIdLookup
{
    private const int MaxStackByteBufferSize = 256;

    private readonly Dictionary<string, ushort> _lookup = new(ReadWriteHelpers.InitialStringListCapacity);
    internal int Count => _lookup.Count;

    public MutableStringIdLookup()
    {
    }

    public static implicit operator StringIdLookup(MutableStringIdLookup lookup) => new(lookup._lookup);

    internal void RecordString(StyleIdentifier styleIdentifier) => RecordString(styleIdentifier.ToString());
    internal void RecordString(PieceIdentifier pieceIdentifier) => RecordString(pieceIdentifier.ToString());
    internal void RecordString(GadgetInputName gadgetInputName) => RecordString(gadgetInputName.ToString());

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

    [SkipLocalsInit]
    internal void WriteStrings<TPerfectHasher, TEnum>(RawFileDataWriter<TPerfectHasher, TEnum> writer)
        where TPerfectHasher : struct, ISectionIdentifierHelper<TEnum>
        where TEnum : unmanaged, Enum
    {
        var bufferSize = CalculateBufferSize();

        FileWritingException.WriterAssert(bufferSize <= IoConstants.MaxStringLengthInBytes, "Cannot serialize a string larger than 2048 bytes!");

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
}
