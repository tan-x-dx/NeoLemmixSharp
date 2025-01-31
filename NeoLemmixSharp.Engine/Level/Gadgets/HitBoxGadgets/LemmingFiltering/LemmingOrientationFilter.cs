using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;

public sealed class LemmingOrientationFilter : ILemmingCriterion
{
    private uint _bits = 0;

    public void RegisterOrientation(Orientation orientation) => BitArrayHelpers.SetBit(new Span<uint>(ref _bits), orientation.RotNum);
    public bool LemmingMatchesCriteria(Lemming lemming) => BitArrayHelpers.GetBit(new ReadOnlySpan<uint>(in _bits), lemming.Orientation.RotNum);
}