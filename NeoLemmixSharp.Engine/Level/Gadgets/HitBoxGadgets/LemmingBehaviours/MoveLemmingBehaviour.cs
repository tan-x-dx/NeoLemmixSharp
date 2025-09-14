using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class MoveLemmingBehaviour : LemmingBehaviour
{
    private readonly Point _deltaPosition;

    public MoveLemmingBehaviour(
        Point deltaPosition)
        : base(LemmingBehaviourType.LemmingMover)
    {
        _deltaPosition = deltaPosition;
    }

    protected override void PerformInternalBehaviour(int triggerData)
    {
        var lemming = GetLemming(triggerData);
        ref var lemmingPosition = ref lemming.AnchorPosition;
        lemmingPosition += _deltaPosition;

        LevelScreen.LemmingManager.UpdateLemmingPosition(lemming);
    }
}
