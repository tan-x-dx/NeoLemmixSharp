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

        var x0 = int.MaxValue;
        var y0 = int.MaxValue;
        var x1 = int.MinValue;
        var y1 = int.MinValue;

        foreach (var kvp in _hitBoxLookup)
        {
            var hitBoxBounds = kvp.Value.CurrentBounds;
            var bottomRight = hitBoxBounds.GetBottomRight();

            x0 = Math.Min(x0, hitBoxBounds.X);
            y0 = Math.Min(y0, hitBoxBounds.Y);
            x1 = Math.Max(x1, bottomRight.X);
            y1 = Math.Max(y1, bottomRight.Y);
        }

        x1 += 1 - x0;
        y1 += 1 - y0;

        x0 += offset.X;
        y0 += offset.Y;

        int* p = stackalloc int[4] { x0, y0, x1, y1 };
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
