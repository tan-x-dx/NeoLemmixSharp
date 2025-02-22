using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Orientations;
using OrientationToHitBoxRegionLookup = NeoLemmixSharp.Common.Util.Collections.BitArrays.BitArrayDictionary<NeoLemmixSharp.Engine.Level.Orientations.Orientation.OrientationHasher, NeoLemmixSharp.Common.Util.Collections.BitArrays.BitBuffer32, NeoLemmixSharp.Engine.Level.Orientations.Orientation, NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes.IHitBoxRegion>;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;

public sealed class GadgetState
{
    private readonly GadgetOutput _stateSelectedOutput = new();
    private readonly LemmingHitBoxFilter[] _lemmingHitBoxFilters;
    private readonly OrientationToHitBoxRegionLookup _hitBoxLookup;

    public GadgetStateAnimationController AnimationController { get; }
    public ReadOnlySpan<LemmingHitBoxFilter> Filters => new(_lemmingHitBoxFilters);

    public GadgetState(
        LemmingHitBoxFilter[] lemmingHitBoxFilters,
        OrientationToHitBoxRegionLookup hitBoxLookup,
        GadgetStateAnimationController animationController)
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

    public LevelRegion GetEncompassingHitBoxBounds(LevelPosition offset)
    {
        if (_hitBoxLookup.Count == 0)
            return new LevelRegion(offset);

        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        foreach (var kvp in _hitBoxLookup)
        {
            var hitBoxBounds = kvp.Value.CurrentBounds;
            var bottomRight = hitBoxBounds.GetBottomRight();

            minX = Math.Min(minX, hitBoxBounds.X);
            minY = Math.Min(minY, hitBoxBounds.Y);
            maxX = Math.Max(maxX, bottomRight.X);
            maxY = Math.Max(maxY, bottomRight.Y);
        }

        minX += offset.X;
        minY += offset.Y;
        maxX += offset.X;
        maxY += offset.Y;

        return new LevelRegion(
            new LevelPosition(minX, minY),
            new LevelSize(1 + maxX - minX, 1 + maxY - minY));
    }

    public void OnTransitionTo()
    {
        AnimationController.OnTransitionTo();
        _stateSelectedOutput.SetSignal(true);
    }

    public void Tick(HitBoxGadget parentGadget)
    {
        var gadgetStateTransitionIndex = AnimationController.Tick();

        if (gadgetStateTransitionIndex != GadgetStateAnimationController.NoGadgetStateTransition)
        {
            parentGadget.SetNextState(gadgetStateTransitionIndex);
        }
    }

    public void OnTransitionFrom()
    {
        _stateSelectedOutput.SetSignal(false);
    }
}
