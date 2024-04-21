using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public interface IControlledAnimationGadget
{
    int Id { get; }
    GadgetStateAnimationController AnimationController { get; }
    RectangularLevelRegion GadgetBounds { get; }
}