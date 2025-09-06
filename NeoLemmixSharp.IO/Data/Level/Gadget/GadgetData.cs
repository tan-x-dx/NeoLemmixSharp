using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Data.Level.Gadget;

public sealed class GadgetData
{
    private readonly BitArrayDictionary<GadgetPropertyTypeHasher, BitBuffer32, GadgetPropertyType, int> _properties = GadgetPropertyTypeHasher.CreateBitArrayDictionary<int>();

    public required GadgetIdentifier Identifier { get; init; }
    public required string OverrideName { get; init; }

    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }

    public required Point Position { get; init; }
    public required int InitialStateId { get; init; }
    public required GadgetRenderMode GadgetRenderMode { get; init; }

    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }

    public required GadgetLayerColorData[] LayerColorData { get; init; }

    public int NumberOfGadgetProperties => _properties.Count;

    internal void AddProperty(GadgetPropertyType property, int value)
    {
        _properties.Add(property, value);
    }

    public int GetProperty(GadgetPropertyType property)
    {
        return _properties[property];
    }

    public bool HasProperty(GadgetPropertyType property)
    {
        return _properties.ContainsKey(property);
    }

    public bool TryGetProperty(GadgetPropertyType property, out int value)
    {
        return _properties.TryGetValue(property, out value);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitArrayDictionary<GadgetPropertyTypeHasher, BitBuffer32, GadgetPropertyType, int>.Enumerator GetProperties() => _properties.GetEnumerator();

    public StylePiecePair GetStylePiecePair() => new(StyleIdentifier, PieceIdentifier);
}
