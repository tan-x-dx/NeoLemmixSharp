using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.Rendering.Viewport;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

public interface IGadgetSpriteBuilder
{
    IViewportObjectRenderer BuildGadgetRenderer(IGadgetBuilder gadgetBuilder, GadgetData gadgetData);
}