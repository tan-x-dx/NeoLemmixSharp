using System.Diagnostics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public sealed class MetalGrateGadget : IGadget
{
    public GadgetType Type { get; }
    public Orientation Orientation { get; }
    public LevelPosition LevelPosition { get; }
    public int AnimationFrame { get; private set; }

    public RectangularLevelRegion SpriteClip { get; }
    public RectangularLevelRegion HitBox { get; }

    private bool _isActive;

    public MetalGrateGadget(
        Orientation orientation,
        LevelPosition levelPosition,
        RectangularLevelRegion renderClip,
        RectangularLevelRegion hitBox)
    {
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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ILevelRegion IGadget.HitBox => HitBox;
}