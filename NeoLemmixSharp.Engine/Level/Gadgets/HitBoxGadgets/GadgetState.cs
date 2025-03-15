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
    public unsafe LevelRegion GetMininmumBoundingBoxForAllHitBoxes(LevelPosition offset)
    {
        if (_hitBoxLookup.Count == 0)
            return new LevelRegion(offset);

        int* p = stackalloc int[4];
        p[0] = int.MaxValue;
        p[1] = int.MaxValue;
        p[2] = int.MinValue;
        p[3] = int.MinValue;

        foreach (var kvp in _hitBoxLookup)
        {
            var hitBoxBounds = kvp.Value.CurrentBounds;
            var bottomRight = hitBoxBounds.GetBottomRight();

            p[0] = Math.Min(p[0], hitBoxBounds.X);
            p[1] = Math.Min(p[1], hitBoxBounds.Y);
            p[2] = Math.Max(p[2], bottomRight.X);
            p[3] = Math.Max(p[3], bottomRight.Y);
        }

        p[2] += 1 - p[0];
        p[3] += 1 - p[1];

        p[0] += offset.X;
        p[1] += offset.Y;

        return *(LevelRegion*)p;
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
