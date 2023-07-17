using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public sealed class EmptyGadgetList<TGadget> : IGadgetCollection<TGadget>
    where TGadget : class, IHitBoxGadget
{
    public static EmptyGadgetList<TGadget> Instance { get; } = new();

    private EmptyGadgetList()
    {
    }

    public bool TryGetGadgetThatMatchesTypeAndOrientation(
        LevelPosition levelPosition,
        Orientation orientation,
        [NotNullWhen(true)] out TGadget? gadget)
    {
        gadget = null;
        return false;
    }
}