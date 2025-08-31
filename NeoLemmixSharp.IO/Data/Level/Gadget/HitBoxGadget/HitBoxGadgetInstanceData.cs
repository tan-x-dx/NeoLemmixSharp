using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.IO.Data.Level.Gadget.HitBoxGadget;

public sealed class HitBoxGadgetInstanceData : IGadgetInstanceData
{
    private readonly BitArrayDictionary<GadgetPropertyHasher, BitBuffer32, GadgetProperty, int> _propertyLookup = new(new());

    public GadgetType GadgetType => GadgetType.HitBoxGadget;
    public required GadgetIdentifier Identifier { get; init; }
    public required GadgetName OverrideName { get; init; }
    public required GadgetLayerColorData[] LayerColorData { get; init; }

    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }

    public required Point Position { get; init; }
    public required int InitialStateId { get; init; }
    public required GadgetRenderMode GadgetRenderMode { get; init; }

    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }
    public required bool IsFastForward { get; init; }

    public required HitBoxGadgetStateInstanceData[] GadgetStates { get; init; }

    public int GetProperty(GadgetProperty gadgetProperty) => _propertyLookup[gadgetProperty];
}
