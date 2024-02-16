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

    public GadgetBase BuildGadget(GadgetData gadgetData, IPerfectHasher<Lemming> lemmingHasher)
    {
        var lemmingCount = gadgetData.GetProperty<int>(GadgetProperty.LemmingCount);

        var spawnPoint = new LevelPosition(SpawnX, SpawnY);

        if (!gadgetData.TryGetProperty<Team>(GadgetProperty.Team, out var team))
        {
            team = Team.AllItems[0];
        }

        if (!gadgetData.TryGetProperty<uint>(GadgetProperty.RawLemmingState, out var rawLemmingState))
        {
            rawLemmingState = 1U << LemmingState.ActiveBitIndex;
        }

        var gadgetBounds = new RectangularLevelRegion(
            gadgetData.X,
            gadgetData.Y,
            HatchWidth,
            HatchHeight);

        var hatchSpawnData = new HatchSpawnData(
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