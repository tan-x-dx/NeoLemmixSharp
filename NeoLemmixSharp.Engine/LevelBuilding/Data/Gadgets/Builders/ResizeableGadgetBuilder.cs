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
    public required GadgetBehaviour GadgetBehaviour { get; init; }

    public required Texture2D Sprite { get; init; }

    public GadgetBase BuildGadget(GadgetData gadgetData, IPerfectHasher<Lemming> lemmingHasher)
    {
        var gadgetWidth = gadgetData.GetProperty(GadgetProperty.Width);
        var gadgetHeight = gadgetData.GetProperty(GadgetProperty.Height);

        var gadgetBounds = new RectangularLevelRegion(
            gadgetData.X,
            gadgetData.Y,
            gadgetWidth,
            gadgetHeight);

        return new ResizeableGadget(
            gadgetData.Id,
            GadgetBehaviour,
            gadgetData.Orientation,
            gadgetBounds,
            new ItemTracker<Lemming>(lemmingHasher));
    }
}