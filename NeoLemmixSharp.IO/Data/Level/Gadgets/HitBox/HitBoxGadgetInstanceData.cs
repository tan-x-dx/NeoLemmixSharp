using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets.HitBox;

public sealed class HitBoxGadgetInstanceData : IGadgetInstanceData
{
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
}
