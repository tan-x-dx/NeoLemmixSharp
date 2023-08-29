using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Collections;

public sealed class EmptyGadgetList : IGadgetCollection
{
    public static EmptyGadgetList Instance { get; } = new();

    private EmptyGadgetList()
    {
    }

    public bool TryGetGadgetThatMatchesTypeAndOrientation(
        Lemming lemming,
        LevelPosition levelPosition,
        [NotNullWhen(true)] out Gadget? gadget)
    {
        gadget = null;
        return false;
    }
}