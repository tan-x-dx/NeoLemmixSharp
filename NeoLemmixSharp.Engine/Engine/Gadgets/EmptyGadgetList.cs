using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;

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
        out TGadget? gadget)
    {
        gadget = null;
        return false;
    }
}