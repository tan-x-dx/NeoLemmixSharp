using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class NoneAction : LemmingAction
{
    /// <summary>
    /// Logically equivalent to null, but null references suck
    /// </summary>
    public static readonly NoneAction Instance = new();

    private NoneAction()
        : base(
            -1,
            EngineConstants.NoneActionName,
            string.Empty,
            1,
            1,
            -1)
    {
    }

    public override bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming)
    {
        return false;
    }

    protected override RectangularRegion ActionBounds() => new();

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
    }
}