using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.SawBlade;

public sealed class SawBladeGadget : GadgetBase, IDestructionMask, IMoveableGadget
{
    private SawBladeHitMask[] _hitMasks;

    public int AnimationFrame { get; private set; }

    public SawBladeGadget(
        int id,
        RectangularLevelRegion gadgetBounds) : base(id, gadgetBounds)
    {
    }

    public override GadgetType Type => GadgetType.SawBlade;
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

    public override IGadgetInput? GetInputWithName(string inputName)
    {
        throw new NotImplementedException();
    }

    public override bool CaresAboutLemmingInteraction => true;

    public override bool MatchesLemming(Lemming lemming)
    {
        var p0 = lemming.LevelPosition;
        var p1 = lemming.Orientation.MoveUp(lemming.LevelPosition, 1);

        var hitMask = _hitMasks[AnimationFrame];

        return hitMask.MatchesPosition(p0) || hitMask.MatchesPosition(p1);
    }

    public override void OnLemmingMatch(Lemming lemming)
    {
        LemmingManager.RemoveLemming(lemming);
    }

    public override bool MatchesPosition(LevelPosition levelPosition) => _hitMasks[AnimationFrame].MatchesPosition(levelPosition);

    public void Move(int dx, int dy)
    {
        GadgetBounds.X += dx;
        GadgetBounds.Y += dy;
    }

    public void SetPosition(int x, int y)
    {
        GadgetBounds.X = x;
        GadgetBounds.Y = y;

        GadgetManager.UpdateGadgetPosition(this);
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