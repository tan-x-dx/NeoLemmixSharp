using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.Orientations;
using System.Diagnostics.Contracts;

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

    [Pure]
    bool MatchesOrientation(LevelPosition levelPosition, Orientation orientation);
    void OnLemmingInHitBox(Lemming lemming);
}
