using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Functional;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class HatchGadgetBuilder : IGadgetBuilder
{
    public required int GadgetBuilderId { get; init; }

    public required int SpawnX { get; init; }
    public required int SpawnY { get; init; }

    public required int HatchWidth { get; init; }
    public required int HatchHeight { get; init; }

    public required Texture2D Sprite { get; init; }

    public GadgetBase BuildGadget(GadgetData gadgetData, IPerfectHasher<Lemming> lemmingHasher)
    {
        var hatchGadgetId = gadgetData.GetProperty<int>(GadgetProperty.HatchGroupId);
        var team = gadgetData.GetProperty<Team>(GadgetProperty.Team);
        var rawLemmingState = gadgetData.GetProperty<uint>(GadgetProperty.RawLemmingState);
        var lemmingCount = gadgetData.GetProperty<int>(GadgetProperty.LemmingCount);

        var spawnPoint = new LevelPosition(SpawnX, SpawnY);

        var gadgetBounds = new RectangularLevelRegion(
            gadgetData.X,
            gadgetData.Y,
            HatchWidth,
            HatchHeight);

        var hatchSpawnData = new HatchSpawnData(
            hatchGadgetId,
            team,
            rawLemmingState,
            gadgetData.Orientation,
            gadgetData.FacingDirection,
            lemmingCount);

        return new HatchGadget(
            gadgetData.Id,
            gadgetBounds,
            spawnPoint,
            hatchSpawnData);
    }
}