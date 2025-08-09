using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
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

    public override void PerformBehaviour(Lemming lemming)
    {
        _action.TransitionLemmingToAction(lemming, false);
    }
}
