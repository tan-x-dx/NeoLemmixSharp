using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.Orientations;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public sealed class ResizeableGadget : IResizableGadget
{
    private int _deltaX;
    private int _deltaY;
    private int _deltaWidth;
    private int _deltaHeight;

    public int Id { get; }
    public GadgetType Type { get; }
    public Orientation Orientation { get; }
    public LevelPosition LevelPosition => SpriteClip.TopLeft;
    public int AnimationFrame { get; private set; }
    public RectangularLevelRegion SpriteClip { get; }
    public RelativeRectangularLevelRegion HitBox { get; }

    public ResizeableGadget(
        int id,
        GadgetType gadgetType,
        Orientation orientation,
        RectangularLevelRegion spriteClip)
    {
        Id = id;
        Type = gadgetType;
        Orientation = orientation;
        SpriteClip = spriteClip;
        HitBox = new RelativeRectangularLevelRegion(spriteClip, 0, 0, 5, 0);
    }

    public void Tick()
    {
        SpriteClip.X += _deltaX;
        SpriteClip.Y += _deltaY;
        SpriteClip.W += _deltaWidth;
        SpriteClip.H += _deltaHeight;

        //  AnimationFrame = (AnimationFrame + 1) & 7;
    }

    public bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation)
    {
        return HitBox.ContainsPoint(levelPosition) ||
               HitBox.ContainsPoint(orientation.MoveUp(levelPosition, 1));
    }

    public void OnLemmingInHitBox(Lemming lemming)
    {
        throw new NotImplementedException();
    }

    public void OnInput(InputType inputType)
    {
        throw new NotImplementedException();
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
    ILevelRegion IHitBoxGadget.HitBox => HitBox;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IRectangularLevelRegion IResizableGadget.HitBox => HitBox;
}