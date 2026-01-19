using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class MoveLemmingBehaviour : LemmingBehaviour
{
    private readonly Point _deltaPosition;

    public MoveLemmingBehaviour(Point deltaPosition)
        : base(LemmingBehaviourType.MoveLemming)
    {
        _deltaPosition = deltaPosition;
    }

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        ref var lemmingPosition = ref lemming.AnchorPosition;
        lemmingPosition += _deltaPosition;

        LevelScreen.LemmingManager.UpdateLemmingPosition(lemming);
    }
}
