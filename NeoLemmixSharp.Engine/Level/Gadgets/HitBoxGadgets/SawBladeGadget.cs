using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class SawBladeGadget : HitBoxGadget, IDestructionMask, IMoveableGadget
{
    private SawBladeHitMask[] _hitMasks;

    public int AnimationFrame { get; private set; }

    public SawBladeGadget(
        int id,
        RectangularLevelRegion gadgetBounds) : base(id, gadgetBounds)
    {
    }

    public override InteractiveGadgetType SubType => SawBladeGadgetType.Instance;
    public override Orientation Orientation => DownOrientation.Instance;

    public void SetHitMasks(SawBladeHitMask[] hitMasks)
    {
        _hitMasks = hitMasks;
    }

    public override void Tick()
    {
        AnimationFrame++;
        if (AnimationFrame == _hitMasks.Length)
        {
            AnimationFrame = 0;
        }

        var hitMask = _hitMasks[AnimationFrame];
        var position = GadgetBounds.TopLeft;
        hitMask.ApplyEraseMask(position);
    }

    public override bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition)
    {
        return _hitMasks[AnimationFrame].MatchesPosition(levelPosition);
    }

    public override bool MatchesPosition(LevelPosition levelPosition) => _hitMasks[AnimationFrame].MatchesPosition(levelPosition);

    public override void OnLemmingMatch(Lemming lemming)
    {
        LevelConstants.LemmingManager.RemoveLemming(lemming);
    }

    public void Move(int dx, int dy)
    {
        var newPosition = TopLeftPixel + new LevelPosition(dx, dy);

        UpdatePosition(newPosition);
    }

    public void SetPosition(int x, int y)
    {
        var newPosition = new LevelPosition(x, y);

        UpdatePosition(newPosition);
    }

    [Pure]
    public bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection)
    {
        // Saw blades do not care about arrows, only if the pixel can be destroyed at all!
        // Since other checks will have already taken place, this code is only ever
        // reached when the pixel can definitely be destroyed by a saw blade.
        // Therefore, just return true.

        return true;
    }
}