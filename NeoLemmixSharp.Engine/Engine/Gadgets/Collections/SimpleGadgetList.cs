using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Collections;

public sealed class SimpleGadgetList<TGadget> : IGadgetCollection<TGadget>
    where TGadget : class, IHitBoxGadget
{
    private readonly TGadget[] _gadgets;

    public SimpleGadgetList(TGadget[] gadgets)
    {
        _gadgets = gadgets;
    }

    public bool TryGetGadgetThatMatchesTypeAndOrientation(
        LevelPosition levelPosition,
        Orientation orientation,
        [NotNullWhen(true)] out TGadget? gadget)
    {
        for (var i = 0; i < _gadgets.Length; i++)
        {
            gadget = _gadgets[i];
            if (gadget.MatchesOrientation(levelPosition, orientation))
                return true;
        }

        gadget = null;
        return false;
    }
}