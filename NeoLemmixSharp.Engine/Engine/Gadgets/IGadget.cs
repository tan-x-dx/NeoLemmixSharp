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

    void Tick();
}

public interface IHitBoxGadget : IGadget
{
    ILevelRegion HitBox { get; }

    bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation);
    void OnLemmingInHitBox(Lemming lemming);
}

public interface IProactiveGadget : IGadget
{

}

public interface IReactiveGadget : IGadget
{
    void OnInput(InputType inputType);
}

public interface IMoveableGadget : IHitBoxGadget, IReactiveGadget
{
    void SetDeltaX(int deltaX);
    void SetDeltaY(int deltaY);
}

public interface IResizableGadget : IHitBoxGadget, IReactiveGadget
{
    new IRectangularLevelRegion HitBox { get; }

    void SetDeltaWidth(int deltaWidth);
    void SetDeltaHeight(int deltaHeight);
}