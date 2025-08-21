using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class SetLemmingActionBehaviour : LemmingBehaviour
{
    private readonly LemmingAction _action;

    public SetLemmingActionBehaviour(
        LemmingAction action)
        : base(LemmingBehaviourType.ChangeLemmingAction)
    {
        _action = action;
    }

    protected override void PerformInternalBehaviour(int lemmingId)
    {
        var lemming = GetLemming(lemmingId);
        _action.TransitionLemmingToAction(lemming, false);
    }
}
