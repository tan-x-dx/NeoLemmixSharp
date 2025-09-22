using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.LemmingActions;

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

    protected override void PerformInternalBehaviour(int triggerData)
    {
        var lemming = GetLemming(triggerData);
        _action.TransitionLemmingToAction(lemming, false);
    }
}
