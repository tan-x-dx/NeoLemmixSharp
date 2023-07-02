using System.Diagnostics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public sealed class ResizeableGadget : IResizableGadget
{
    private int _deltaX;
    private int _deltaY;
    private int _deltaWidth;
    private int _deltaHeight;

    public GadgetType Type { get; }
    public Orientation Orientation { get; }
    public LevelPosition LevelPosition { get; }
    public int AnimationFrame { get; }
    public RectangularLevelRegion SpriteClip { get; }
    public RelativeRectangularLevelRegion HitBox { get; }

    public ResizeableGadget(
        GadgetType gadgetType,
        Orientation orientation)
    {
        Type = gadgetType;
        Orientation = orientation;
    }

    public void Tick()
    {
        SpriteClip.X += _deltaX;
        SpriteClip.Y += _deltaY;
        SpriteClip.W += _deltaWidth;
        SpriteClip.H += _deltaHeight;
    }

    public bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation)
    {
        var offset = levelPosition - LevelPosition;

        return HitBox.ContainsPoint(offset) ||
               HitBox.ContainsPoint(orientation.MoveUp(offset, 1));
    }

    public void SetDeltaX(int deltaX)
    {
        _deltaX = Math.Clamp(deltaX, -10, 10);
    }

    public void SetDeltaY(int deltaY)
    {
        _deltaY = Math.Clamp(deltaY, -10, 10);
    }

    public void SetDeltaWidth(int deltaWidth)
    {
        _deltaWidth = Math.Clamp(deltaWidth, -10, 10);
    }

    public void SetDeltaHeight(int deltaHeight)
    {
        _deltaHeight = Math.Clamp(deltaHeight, -10, 10);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ILevelRegion IGadget.HitBox => HitBox;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IRectangularLevelRegion IResizableGadget.HitBox => HitBox;
}