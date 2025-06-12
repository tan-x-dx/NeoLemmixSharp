using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class LemmingMoverAction : GadgetAction
{
    private readonly Point _deltaPosition;

    public LemmingMoverAction(Point deltaPosition)
        : base(GadgetActionType.LemmingMover)
    {
        _deltaPosition = deltaPosition;
    }

    public override void PerformAction(Lemming lemming)
    {
        ref var lemmingPosition = ref lemming.AnchorPosition;
        lemmingPosition += _deltaPosition;

        LevelScreen.LemmingManager.UpdateLemmingPosition(lemming);
    }
}
