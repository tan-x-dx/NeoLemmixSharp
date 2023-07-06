using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public sealed class MoveableGadget : IMoveableGadget
{
    private int _deltaX;
    private int _deltaY;

    public int Id { get; }
    public GadgetType Type { get; }
    public Orientation Orientation { get; }
    public LevelPosition LevelPosition { get; }

    public int AnimationFrame { get; private set; }

    public RectangularLevelRegion SpriteClip { get; }
    public ILevelRegion HitBox { get; }

    public MoveableGadget(
        int id,
        GadgetType gadgetType,
        Orientation orientation)
    {
        Id = id;
        Type = gadgetType;
        Orientation = orientation;
    }

    public void Tick()
    {
        SpriteClip.X += _deltaX;
        SpriteClip.Y += _deltaY;
    }

    public bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation)
    {
        var offset = levelPosition - LevelPosition;

        return HitBox.ContainsPoint(offset) ||
               HitBox.ContainsPoint(orientation.MoveUp(offset, 1));
    }

    public void OnLemmingInHitBox(Lemming lemming)
    {

    }

    public void SetDeltaX(int deltaX)
    {
        _deltaX = Math.Clamp(deltaX, -10, 10);
    }

    public void SetDeltaY(int deltaY)
    {
        _deltaY = Math.Clamp(deltaY, -10, 10);
    }
}