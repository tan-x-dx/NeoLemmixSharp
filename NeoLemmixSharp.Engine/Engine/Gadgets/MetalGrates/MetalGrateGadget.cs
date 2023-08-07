using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.Gadgets.States;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.MetalGrates;

public sealed class MetalGrateGadget : IGadget
{
    public int Id { get; }
    public GadgetType Type { get; }
    public Orientation Orientation { get; }
    public LevelPosition LevelPosition { get; }
    public IGadgetState CurrentState { get; }
    public int AnimationFrame { get; private set; }

    public RectangularLevelRegion SpriteClip { get; }
    public IViewportObjectRenderer Renderer { get; }
    public RectangularLevelRegion HitBox { get; }

    private bool _isActive;

    public MetalGrateGadget(
        int id,
        Orientation orientation,
        LevelPosition levelPosition,
        RectangularLevelRegion renderClip,
        RectangularLevelRegion hitBox)
    {
        Id = id;
        Orientation = orientation;
        LevelPosition = levelPosition;
        SpriteClip = renderClip;
        HitBox = hitBox;
    }

    public void Activate()
    {
        _isActive = true;
    }

    public void Deactivate()
    {
        _isActive = false;
    }

    public void Tick()
    {
    }

    public bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation)
    {
        if (!_isActive)
            return false;

        var offset = levelPosition - SpriteClip.TopLeft;

        return HitBox.ContainsPoint(offset);
    }

    public void OnLemmingInHitBox(Lemming lemming)
    {

    }
}