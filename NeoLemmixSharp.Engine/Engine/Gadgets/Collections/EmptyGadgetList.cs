using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Collections;

public sealed class EmptyGadgetList<TGadget> : IGadgetCollection<TGadget>
    where TGadget : class, IGadget
{
    public static EmptyGadgetList<TGadget> Instance { get; } = new();

    private EmptyGadgetList()
    {
    }

    public bool TryGetGadgetThatMatchesTypeAndOrientation(
        Lemming lemming,
        LevelPosition levelPosition,
        [NotNullWhen(true)] out TGadget? gadget)
    {
        gadget = null;
        return false;
    }
}