using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

public sealed class GadgetData
{
    private readonly BitArrayDictionary<GadgetPropertyHasher, BitBuffer32, GadgetProperty, int> _properties = GadgetPropertyHasher.CreateBitArrayDictionary<int>();

    public required int Id { get; init; }
    public required string OverrideName { get; init; }

    public required StyleIdentifier StyleName { get; init; }
    public required PieceIdentifier PieceName { get; init; }

    public required Point Position { get; init; }
    public required int InitialStateId { get; init; }
    public required GadgetRenderMode GadgetRenderMode { get; init; }

    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }

    public required GadgetInputName[] OverrideInputNames { get; init; }
    public required GadgetLayerColorData[] LayerColorData { get; init; }

    public int NumberOfGadgetProperties => _properties.Count;

    public void AddProperty(GadgetProperty property, int value)
    {
        _properties.Add(property, value);
    }

    public int GetProperty(GadgetProperty property)
    {
        return _properties[property];
    }

    public bool HasProperty(GadgetProperty property)
    {
        return _properties.ContainsKey(property);
    }

    public bool TryGetProperty(GadgetProperty property, out int value)
    {
        return _properties.TryGetValue(property, out value);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitArrayDictionary<GadgetPropertyHasher, BitBuffer32, GadgetProperty, int>.Enumerator GetProperties() => _properties.GetEnumerator();

    public StylePiecePair GetStylePiecePair() => new(StyleName, PieceName);
}