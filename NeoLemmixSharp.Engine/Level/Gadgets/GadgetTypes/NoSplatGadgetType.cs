﻿using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class NoSplatGadgetType : InteractiveGadgetType
{
    public static NoSplatGadgetType Instance { get; } = new();

    private NoSplatGadgetType()
    {
    }

    public override int Id => Global.NoSplatGadgetTypeId;
    public override string GadgetTypeName => "no splat";

    public override LemmingAction InteractWithLemming(Lemming lemming)
    {
        return NoneAction.Instance;
    }
}