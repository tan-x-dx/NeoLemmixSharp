using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoLemmixSharp.IO.Writing;

internal readonly struct FileWriterStringIdLookup
{
    private readonly Dictionary<string, ushort> _lookup;

    internal FileWriterStringIdLookup(Dictionary<string, ushort> lookup) => _lookup = lookup;

    [Pure]
    internal ushort GetStringId(StyleIdentifier styleIdentifier) => GetStringId(styleIdentifier.ToString());
    [Pure]
    internal ushort GetStringId(PieceIdentifier pieceIdentifier) => GetStringId(pieceIdentifier.ToString());
    [Pure]
    internal ushort GetStringId(GadgetTriggerName gadgetInputName) => GetStringId(gadgetInputName.ToString());
    [Pure]
    internal ushort GetStringId(GadgetStateName gadgetStateName) => GetStringId(gadgetStateName.ToString());

    [Pure]
    internal ushort GetStringId(string? s)
    {
        if (string.IsNullOrEmpty(s))
            return 0;

        return _lookup[s];
    }
}

internal readonly struct MutableFileWriterStringIdLookup
{
    private const int MaxStackByteBufferSize = 256;

    private readonly Dictionary<string, ushort> _lookup = new(IoConstants.InitialStringListCapacity);
    internal int Count => _lookup.Count;

    public MutableFileWriterStringIdLookup()
    {
    }

    public static implicit operator FileWriterStringIdLookup(MutableFileWriterStringIdLookup lookup) => new(lookup._lookup);

    internal void RecordString(StyleIdentifier styleIdentifier) => RecordString(styleIdentifier.ToString());
    internal void RecordString(PieceIdentifier pieceIdentifier) => RecordString(pieceIdentifier.ToString());
    internal void RecordString(GadgetTriggerName gadgetInputName) => RecordString(gadgetInputName.ToString());
    internal void RecordString(GadgetStateName gadgetStateName) => RecordString(gadgetStateName.ToString());

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
    internal void WriteStrings<TWriter>(TWriter writer)
        where TWriter : class, IRawFileDataWriter
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

            writer.Write16BitUnsignedInteger(id);

            var byteCount = Encoding.UTF8.GetBytes(stringToWrite, buffer);

            writer.Write16BitUnsignedInteger((ushort)byteCount);
            writer.WriteBytes(buffer[..byteCount]);
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
