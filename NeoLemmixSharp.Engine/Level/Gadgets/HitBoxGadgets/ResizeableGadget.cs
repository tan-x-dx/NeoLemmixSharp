using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class ResizeableGadget : HitBoxGadget, IMoveableGadget, IResizeableGadget
{
    private readonly HitBox _hitBox;

    public override GadgetBehaviour GadgetBehaviour { get; }
    public override Orientation Orientation { get; }

    public ResizeableGadget(
        int id,
        GadgetBehaviour gadgetBehaviour,
        Orientation orientation,
        RectangularLevelRegion gadgetBounds,
        INineSliceGadgetRender? renderer,
        ItemTracker<Lemming> lemmingTracker,
        HitBox hitBox)
        : base(id, gadgetBounds, renderer, lemmingTracker)
    {
        GadgetBehaviour = gadgetBehaviour;
        Orientation = orientation;
        _hitBox = hitBox;
    }

    public override void Tick()
    {
    }

    public override bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition)
    {
        return _hitBox.MatchesLemming(lemming) &&
               _hitBox.MatchesPosition(levelPosition);
    }

    public override bool MatchesPosition(LevelPosition levelPosition)
    {
        return _hitBox.MatchesPosition(levelPosition);
    }

    public override bool IsSolidToLemmingAtPosition(Lemming lemming, LevelPosition levelPosition)
    {
        return false;
    }

    public override bool IsSteelToLemmingAtPosition(Lemming lemming, LevelPosition levelPosition)
    {
        return false;
    }

    public override void OnLemmingMatch(Lemming lemming)
    {
        //    throw new NotImplementedException();
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

    public void Resize(int dw, int dh)
    {
        var newSize = new LevelPosition(GadgetBounds.W + dw, GadgetBounds.H + dh);

        UpdateSize(newSize);
    }

    public void SetSize(int w, int h)
    {
        var newSize = new LevelPosition(w, h);

        UpdateSize(newSize);
    }
}