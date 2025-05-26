using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingActionCriterion : LemmingCriterion
{
    private LemmingAction.LemmingActionBitBuffer _lemmingActionBits;

    public LemmingActionCriterion()
        : base(LemmingCriteria.LemmingAction)
    {
    }

    public void RegisterActions(uint[] actionIds)
    {
     //   actions.WriteTo(_lemmingActionBits);
    }

    public override bool LemmingMatchesCriteria(Lemming lemming)
    {
        ReadOnlySpan<uint> bits = _lemmingActionBits;
        var actionComparer = new LemmingAction.LemmingActionHasher();
        return BitArrayHelpers.GetBit(bits, actionComparer.Hash(lemming.CurrentAction));
    }
}