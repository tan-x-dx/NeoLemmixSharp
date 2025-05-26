using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class SetLemmingActionAction : GadgetAction
{
    private readonly LemmingAction _action;

    public SetLemmingActionAction(LemmingAction action)
        : base(GadgetActionType.SetLemmingAction)
    {
        _action = action;
    }

    public override void PerformAction(Lemming lemming)
    {
        _action.TransitionLemmingToAction(lemming, false);
    }
}
