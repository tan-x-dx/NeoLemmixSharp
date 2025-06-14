﻿using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class SetLemmingActionAction : GadgetAction
{
    private readonly LemmingAction _action;

    public SetLemmingActionAction(LemmingAction action)
        : base(GadgetActionType.ChangeLemmingAction)
    {
        _action = action;
    }

    public override void PerformAction(Lemming lemming)
    {
        _action.TransitionLemmingToAction(lemming, false);
    }
}
