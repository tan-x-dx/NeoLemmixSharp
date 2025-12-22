using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
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
    internal ushort GetStringId(GadgetName gadgetInputName) => GetStringId(gadgetInputName.ToString());
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
    internal void RecordString(GadgetName gadgetInputName) => RecordString(gadgetInputName.ToString());
    internal void RecordString(GadgetTriggerName gadgetInputName) => RecordString(gadgetInputName.ToString());
    internal void RecordString(GadgetStateName gadgetStateName) => RecordString(gadgetStateName.ToString());

    internal void RecordString(string? s)
    {
        if (string.IsNullOrEmpty(s))
            return;

        ref var correspondingStringId = ref CollectionsMarshal.GetValueRefOrAddDefault(_lookup, s, out var exists);
        if (!exists)
        {
            // IDs start at 1, ID 0 is associated with the empty string
            // We don't serialise the empty string though
            correspondingStringId = (ushort)_lookup.Count;
        }
    }

    [SkipLocalsInit]
    internal void WriteStrings<TWriter>(TWriter writer)
        where TWriter : class, IRawFileDataWriter
    {
        var bufferSize = CalculateBufferSize();

        FileWritingException.WriterAssert(bufferSize <= IoConstants.MaxStringLengthInBytes, "Cannot serialize a string larger than 2048 bytes!");

        Span<byte> buffer = (uint)bufferSize <= MaxStackByteBufferSize
            ? stackalloc byte[bufferSize]
            : new byte[bufferSize];

        var orderedPairs = GetOrderedPairs();

        foreach (var kvp in orderedPairs)
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
            var newByteCount = Encoding.UTF8.GetByteCount(kvp.Key);
            if (newByteCount > maxBufferSize)
                maxBufferSize = newByteCount;
        }

        return maxBufferSize;
    }

    [Pure]
    private Span<KeyValuePair<string, ushort>> GetOrderedPairs()
    {
        var array = _lookup.ToArray();
        var span = new Span<KeyValuePair<string, ushort>>(array);
        span.Sort(ByValue);
        return span;

        static int ByValue(KeyValuePair<string, ushort> x, KeyValuePair<string, ushort> y)
        {
            int xValue = x.Value;
            int yValue = y.Value;
            return xValue.CompareTo(yValue);
        }
    }
}
