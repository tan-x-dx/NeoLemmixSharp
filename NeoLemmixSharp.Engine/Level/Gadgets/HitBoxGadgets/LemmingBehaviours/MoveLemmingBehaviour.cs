using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;

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

    protected override void PerformInternalBehaviour(int lemmingId)
    {
        var lemming = GetLemming(lemmingId);
        ref var lemmingPosition = ref lemming.Data.AnchorPosition;
        lemmingPosition += _deltaPosition;

        LevelScreen.LemmingManager.UpdateLemmingPosition(lemming);
    }
}
