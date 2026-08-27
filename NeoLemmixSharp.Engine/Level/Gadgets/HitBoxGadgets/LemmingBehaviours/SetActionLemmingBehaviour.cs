using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class SetActionLemmingBehaviour : LemmingBehaviour
{
    private readonly LemmingActionType _actionType;

    public SetActionLemmingBehaviour(
        LemmingActionType actionType)
        : base(LemmingBehaviourType.SetLemmingAction)
    {
        _actionType = actionType;
    }

    protected override void PerformInternalBehaviour(Lemming lemming)
    {
        LemmingAction.TransitionLemmingToAction(lemming, false, _actionType);
    }
}
