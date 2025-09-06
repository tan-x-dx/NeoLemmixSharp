using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.Functional;

public sealed class LevelTimerObserverGadgetArchetypeData : IGadgetArchetypeData
{
    public GadgetType GadgetType => GadgetType.LevelTimerObserver;
    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required GadgetName GadgetName { get; init; }
    public required Size BaseSpriteSize { get; init; }
}
