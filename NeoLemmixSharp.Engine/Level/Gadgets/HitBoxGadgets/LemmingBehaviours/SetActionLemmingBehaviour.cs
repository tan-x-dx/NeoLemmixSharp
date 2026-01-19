using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

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

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        _action.TransitionLemmingToAction(lemming, false);
    }
}
