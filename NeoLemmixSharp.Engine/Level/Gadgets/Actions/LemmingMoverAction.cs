using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class LemmingMoverAction : IGadgetAction
{
    private readonly Point _deltaPosition;

    public LemmingMoverAction(Point deltaPosition)
    {
        _deltaPosition = deltaPosition;
    }

    public void PerformAction(Lemming lemming)
    {
        ref var lemmingPosition = ref lemming.AnchorPosition;
        lemmingPosition += _deltaPosition;

        LevelScreen.LemmingManager.UpdateLemmingPosition(lemming);
    }
}
