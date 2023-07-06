using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public interface IGadget
{
    int Id { get; }
    GadgetType Type { get; }
    Orientation Orientation { get; }
    LevelPosition LevelPosition { get; }
    int AnimationFrame { get; }

    RectangularLevelRegion SpriteClip { get; }
    ILevelRegion HitBox { get; }

    void Tick();
    bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation);
    void OnLemmingInHitBox(Lemming lemming);
}

public interface IMoveableGadget : IGadget
{
    void SetDeltaX(int deltaX);
    void SetDeltaY(int deltaY);
}

public interface IResizableGadget : IMoveableGadget
{
    new IRectangularLevelRegion HitBox { get; }

    void SetDeltaWidth(int deltaWidth);
    void SetDeltaHeight(int deltaHeight);
}