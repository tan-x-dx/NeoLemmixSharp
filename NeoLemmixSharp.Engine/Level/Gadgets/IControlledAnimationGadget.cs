using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public interface IControlledAnimationGadget
{
    GadgetStateAnimationController AnimationController { get; }
    RectangularLevelRegion GadgetBounds { get; }
}