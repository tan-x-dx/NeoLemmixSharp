using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

public sealed class GadgetData
{
    private readonly BitArrayDictionary<GadgetPropertyHasher, BitBuffer32, GadgetProperty, int> _properties = GadgetPropertyHasher.CreateBitArrayDictionary<int>();

    public required GadgetIdentifier Identifier { get; init; }
    public required string OverrideName { get; init; }

    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }

    public required Point Position { get; init; }
    public required int InitialStateId { get; init; }
    public required GadgetRenderMode GadgetRenderMode { get; init; }

    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }

    public required GadgetInputName[] OverrideInputNames { get; init; }
    public required GadgetLayerColorData[] LayerColorData { get; init; }
    public required HitBoxCriteriaData? OverrideHitBoxCriteriaData { get; init; }

    public int NumberOfGadgetProperties => _properties.Count;

    internal void AddProperty(GadgetProperty property, int value)
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

    public StylePiecePair GetStylePiecePair() => new(StyleIdentifier, PieceIdentifier);
}