using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public sealed class GadgetData
{
    private readonly BitArrayDictionary<GadgetPropertyHasher, BitBuffer32, GadgetProperty, int> _properties = GadgetPropertyHasher.CreateBitArrayDictionary<int>();

    public required int Id { get; init; }

    public required string Style { get; init; }
    public required string GadgetPiece { get; init; }

    public required LevelPosition Position { get; init; }
    public required int InitialStateId { get; init; }
    public required GadgetRenderMode GadgetRenderMode { get; init; }

    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }

    public required string[] InputNames { get; init; }

    public StylePiecePair GetStylePiecePair() => new(Style, GadgetPiece);

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
}