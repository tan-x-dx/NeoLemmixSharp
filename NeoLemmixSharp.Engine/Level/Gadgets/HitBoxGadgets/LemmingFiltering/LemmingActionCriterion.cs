using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingActionCriterion : ILemmingCriterion
{
    private LemmingAction.LemmingActionBitBuffer _lemmingActionBits;

    public void RegisterActions(LemmingActionSet actions)
    {
        actions.WriteTo(_lemmingActionBits);
    }

    public bool LemmingMatchesCriteria(Lemming lemming)
    {
        ReadOnlySpan<uint> bits = _lemmingActionBits;
        var actionComparer = new LemmingAction.LemmingActionHasher();
        return BitArrayHelpers.GetBit(bits, actionComparer.Hash(lemming.CurrentAction));
    }
}