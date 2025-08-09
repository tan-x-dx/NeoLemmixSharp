using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class LemmingMoverBehaviour : LemmingBehaviour
{
    private readonly Point _deltaPosition;

    public LemmingMoverBehaviour(
        Point deltaPosition)
        : base(LemmingBehaviourType.LemmingMover)
    {
        _deltaPosition = deltaPosition;
    }

    public override void PerformBehaviour(Lemming lemming)
    {
        ref var lemmingPosition = ref lemming.AnchorPosition;
        lemmingPosition += _deltaPosition;

        LevelScreen.LemmingManager.UpdateLemmingPosition(lemming);
    }
}
