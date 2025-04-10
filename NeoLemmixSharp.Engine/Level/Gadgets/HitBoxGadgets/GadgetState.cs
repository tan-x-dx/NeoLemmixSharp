using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using System.Runtime.CompilerServices;
using OrientationToHitBoxRegionLookup = NeoLemmixSharp.Common.Util.Collections.BitArrays.BitArrayDictionary<NeoLemmixSharp.Common.Orientation.OrientationHasher, NeoLemmixSharp.Common.Util.Collections.BitArrays.BitBuffer32, NeoLemmixSharp.Common.Orientation, NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes.IHitBoxRegion>;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class GadgetState
{
    private readonly GadgetOutput _stateSelectedOutput = new();
    private readonly LemmingHitBoxFilter[] _lemmingHitBoxFilters;
    private readonly OrientationToHitBoxRegionLookup _hitBoxLookup;

    public AnimationController AnimationController { get; }
    public ReadOnlySpan<LemmingHitBoxFilter> Filters => new(_lemmingHitBoxFilters);

    public GadgetState(
        LemmingHitBoxFilter[] lemmingHitBoxFilters,
        OrientationToHitBoxRegionLookup hitBoxLookup,
        AnimationController animationController)
    {
        _lemmingHitBoxFilters = lemmingHitBoxFilters;
        _hitBoxLookup = hitBoxLookup;
        AnimationController = animationController;
    }

    public IHitBoxRegion HitBoxFor(Orientation orientation)
    {
        if (_hitBoxLookup.TryGetValue(orientation, out var hitBoxRegion))
            return hitBoxRegion;
        return EmptyHitBoxRegion.Instance;
    }

    [SkipLocalsInit]
    public unsafe RectangularRegion GetMininmumBoundingBoxForAllHitBoxes(Point offset)
    {
        if (_hitBoxLookup.Count == 0)
            return new RectangularRegion(offset);

        var p0 = int.MaxValue;
        var p1 = int.MaxValue;
        var p2 = int.MinValue;
        var p3 = int.MinValue;

        foreach (var kvp in _hitBoxLookup)
        {
            var hitBoxBounds = kvp.Value.CurrentBounds;
            var bottomRight = hitBoxBounds.GetBottomRight();

            p0 = Math.Min(p0, hitBoxBounds.X);
            p1 = Math.Min(p1, hitBoxBounds.Y);
            p2 = Math.Max(p2, bottomRight.X);
            p3 = Math.Max(p3, bottomRight.Y);
        }

        p2 += 1 - p0;
        p3 += 1 - p1;

        p0 += offset.X;
        p1 += offset.Y;

        int* p = stackalloc int[4] { p0, p1, p2, p3 };
        return *(RectangularRegion*)p;
    }

    public void OnTransitionTo()
    {
        AnimationController.OnTransitionTo();
        _stateSelectedOutput.SetSignal(true);
    }

    public void Tick(HitBoxGadget parentGadget)
    {
        AnimationController.Tick();

        var nextStateIndex = AnimationController.GetNextStateIndex();
        if (nextStateIndex >= 0)
        {
            parentGadget.SetNextState(nextStateIndex);
        }
    }

    public void OnTransitionFrom()
    {
        _stateSelectedOutput.SetSignal(false);
    }
}
