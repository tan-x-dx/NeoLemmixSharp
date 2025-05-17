using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoLemmixSharp.IO.Writing;

internal readonly struct StringIdLookup
{
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

    /// <summary>
    /// Returns a sequence of string/ID pairs, in ascending ID order
    /// </summary>
    internal IEnumerable<KeyValuePair<string, ushort>> OrderedPairs => _lookup
        .OrderBy(x => x.Value);

    [Pure]
    internal int CalculateBufferSize()
    {
        var maxBufferSize = 0;

        foreach (var kvp in _lookup)
        {
            maxBufferSize = Math.Max(maxBufferSize, Encoding.UTF8.GetByteCount(kvp.Key));
        }

        return maxBufferSize;
    }
}
