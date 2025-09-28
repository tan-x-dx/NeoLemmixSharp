using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public sealed class GadgetArchetypeData
{
    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required GadgetName GadgetName { get; init; }

    public required Size BaseSpriteSize { get; init; }

    public required IGadgetArchetypeSpecificationData SpecificationData { get; init; }
}
