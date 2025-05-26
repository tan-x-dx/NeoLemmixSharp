using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingOrientationCriterion : LemmingCriterion
{
    private uint _bits = 0;

    public LemmingOrientationCriterion()
        : base(LemmingCriteria.LemmingOrientation)
    {
    }

    public void RegisterOrientation(Orientation orientation) => BitArrayHelpers.SetBit(new Span<uint>(ref _bits), orientation.RotNum);
    public override bool LemmingMatchesCriteria(Lemming lemming) => BitArrayHelpers.GetBit(new ReadOnlySpan<uint>(in _bits), lemming.Orientation.RotNum);
}