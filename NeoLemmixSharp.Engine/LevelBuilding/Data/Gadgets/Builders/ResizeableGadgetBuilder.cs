using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class ResizeableGadgetBuilder : IGadgetBuilder
{
    public required int GadgetBuilderId { get; init; }

    public required Texture2D Sprite { get; init; }

    public GadgetBase BuildGadget(GadgetData gadgetData, IPerfectHasher<Lemming> lemmingHasher)
    {
        var gadgetBehaviour = gadgetData.GetProperty<GadgetBehaviour>(GadgetProperty.Behaviour);
        var gadgetWidth = gadgetData.GetProperty<int>(GadgetProperty.Width);
        var gadgetHeight = gadgetData.GetProperty<int>(GadgetProperty.Height);

        var gadgetBounds = new RectangularLevelRegion(
            gadgetData.X,
            gadgetData.Y,
            gadgetWidth,
            gadgetHeight);

        return new ResizeableGadget(
            gadgetData.Id,
            gadgetBehaviour,
            gadgetData.Orientation,
            gadgetBounds,
            new ItemTracker<Lemming>(lemmingHasher));
    }
}