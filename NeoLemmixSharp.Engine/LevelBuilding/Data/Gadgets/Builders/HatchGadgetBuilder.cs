using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Functional;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class HatchGadgetBuilder : IGadgetBuilder
{
    public required int GadgetBuilderId { get; init; }

    public required int SpawnX { get; init; }
    public required int SpawnY { get; init; }

    public required SpriteData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetSpriteBankBuilder gadgetSpriteBankBuilder,
        GadgetData gadgetData,
        IPerfectHasher<Lemming> lemmingHasher)
    {
        var hatchGadgetId = gadgetData.GetProperty(GadgetProperty.HatchGroupId);
        var teamId = gadgetData.GetProperty(GadgetProperty.TeamId);
        var rawLemmingState = (uint)gadgetData.GetProperty(GadgetProperty.RawLemmingState);
        var lemmingCount = gadgetData.GetProperty(GadgetProperty.LemmingCount);

        var dihedralTransformation = new DihedralTransformation(
            gadgetData.Orientation.RotNum,
            false); // Hatches do not flip according to facing direction

        dihedralTransformation.Transform(
            SpriteData.SpriteWidth,
            SpriteData.SpriteHeight,
            SpriteData.SpriteWidth,
            SpriteData.SpriteHeight,
            out var transformedWidth,
            out var transformedHeight);

        dihedralTransformation.Transform(
            SpawnX,
            SpawnY,
            SpriteData.SpriteWidth,
            SpriteData.SpriteHeight,
            out var transformedSpawnX,
            out var transformedSpawnY);

        var spawnPoint = new LevelPosition(transformedSpawnX, transformedSpawnY);

        var gadgetBounds = new RectangularLevelRegion(
            gadgetData.X,
            gadgetData.Y,
        transformedWidth,
            transformedHeight);

        var gadgetRenderer = gadgetSpriteBankBuilder.BuildGadgetRenderer(this, gadgetData);

        var hatchSpawnData = new HatchSpawnData(
            hatchGadgetId,
            Team.AllItems[teamId],
            rawLemmingState,
            gadgetData.Orientation,
            gadgetData.FacingDirection,
            lemmingCount);

        return new HatchGadget(
            gadgetData.Id,
            gadgetBounds,
            gadgetRenderer,
            spawnPoint,
            hatchSpawnData);
    }
}