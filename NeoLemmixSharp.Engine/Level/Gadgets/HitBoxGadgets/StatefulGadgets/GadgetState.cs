using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;

public sealed class GadgetState
{
    private readonly GadgetOutput _stateSelectedOutput = new();
    private readonly LemmingHitBoxFilter[] _lemmingHitBoxFilters;
    private readonly SimpleDictionary<OrientationComparer, Orientation, IHitBoxRegion> _hitBoxLookup;

    public GadgetStateAnimationController AnimationController { get; }
    public ReadOnlySpan<LemmingHitBoxFilter> Filters => new(_lemmingHitBoxFilters);

    public GadgetState(
        LemmingHitBoxFilter[] lemmingHitBoxFilters,
        SimpleDictionary<OrientationComparer, Orientation, IHitBoxRegion> hitBoxLookup,
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

    public LevelPosition TopLeftHitBoxPosition()
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;

        foreach (var (_, hitBox) in _hitBoxLookup)
        {
            var topLeftPosition = hitBox.Offset;

            minX = Math.Min(minX, topLeftPosition.X);
            minY = Math.Min(minY, topLeftPosition.Y);
        }

        return new LevelPosition(minX, minY);
    }

    public LevelPosition BottomRightHitBoxPosition()
    {
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        foreach (var (_, hitBox) in _hitBoxLookup)
        {
            var bottomRightPosition = hitBox.Offset + hitBox.BoundingBoxDimensions;

            maxX = Math.Max(maxX, bottomRightPosition.X);
            maxY = Math.Max(maxY, bottomRightPosition.Y);
        }

        return new LevelPosition(maxX, maxY);
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
