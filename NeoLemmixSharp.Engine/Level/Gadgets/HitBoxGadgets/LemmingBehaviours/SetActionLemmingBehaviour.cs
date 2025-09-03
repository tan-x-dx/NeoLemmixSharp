using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class SetActionLemmingBehaviour : LemmingBehaviour
{
    private readonly LemmingAction _action;

    public SetActionLemmingBehaviour(
        LemmingAction action)
        : base(LemmingBehaviourType.SetLemmingAction)
    {
        _action = action;
    }

    protected override void PerformInternalBehaviour(int lemmingId)
    {
        var lemming = GetLemming(lemmingId);
        _action.TransitionLemmingToAction(lemming, false);
    }
}
