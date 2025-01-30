using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.LemmingActions.LemmingAction;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingActionCriterion : ILemmingCriterion
{
    private LemmingActionBitBuffer _lemmingActionBits;

    public void RegisterActions(LemmingActionSet actions)
    {
        actions.WriteTo(_lemmingActionBits);
    }

    public bool LemmingMatchesCriteria(Lemming lemming)
    {
        ReadOnlySpan<uint> bits = _lemmingActionBits;
        var actionComparer = new LemmingActionComparer();
        return BitArrayHelpers.GetBit(bits, actionComparer.Hash(lemming.CurrentAction));
    }
}