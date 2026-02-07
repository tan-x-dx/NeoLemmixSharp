using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.IO.Data.Level.Gadget;

public sealed class GadgetInstanceData : IInstanceData
{
    public required GadgetIdentifier Identifier { get; init; }
    public required GadgetName OverrideName { get; init; }

    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }

    public required Point Position { get; set; }

    public Size Size => default;//SpecificationData.;
    public required GadgetRenderMode GadgetRenderMode { get; init; }

    public required Orientation Orientation { get; set; }
    public required FacingDirection FacingDirection { get; set; }
    public required bool IsFastForward { get; init; }

    public required IGadgetInstanceSpecificationData SpecificationData { get; init; }

    public StylePiecePair GetStylePiecePair() => new(StyleIdentifier, PieceIdentifier);
}
