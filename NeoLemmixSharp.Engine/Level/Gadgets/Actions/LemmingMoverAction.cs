using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class LemmingMoverAction : IGadgetAction
{
    private readonly LevelPosition _deltaPosition;

    public LemmingMoverAction(LevelPosition deltaPosition)
    {
        _deltaPosition = deltaPosition;
    }

    public void PerformAction(Lemming lemming)
    {
        ref var lemmingPosition = ref lemming.LevelPosition;
        lemmingPosition += _deltaPosition;

        LevelScreen.LemmingManager.UpdateLemmingPosition(lemming);
    }
}
