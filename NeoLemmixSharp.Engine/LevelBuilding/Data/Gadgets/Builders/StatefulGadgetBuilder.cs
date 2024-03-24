using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class StatefulGadgetBuilder : IGadgetBuilder
{
    public required int GadgetBuilderId { get; init; }
    public required GadgetBehaviour GadgetBehaviour { get; init; }
    public required GadgetStateArchetypeData[] AllGadgetStateData { get; init; }

    public required SpriteData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        IPerfectHasher<Lemming> lemmingHasher)
    {
        var bounds = new RectangularLevelRegion(
            gadgetData.X,
            gadgetData.Y,
            SpriteData.SpriteWidth,
            SpriteData.SpriteHeight);

        var gadgetStates = CreateStates(gadgetData);
        var gadgetRenderer = gadgetSpriteBuilder.BuildGadgetRenderer(this, gadgetData);

        var result = new StatefulGadget(
            gadgetData.Id,
            gadgetData.Orientation,
            bounds,
            gadgetRenderer,
            gadgetStates,
            new ItemTracker<Lemming>(lemmingHasher));

        result.SetNextState(gadgetData.InitialStateId);

        gadgetRenderer?.SetGadget(result);

        return result;
    }

    private GadgetState[] CreateStates(GadgetData gadgetData)
    {
        var result = new GadgetState[AllGadgetStateData.Length];

        for (var i = AllGadgetStateData.Length - 1; i >= 0; i--)
        {
            result[i] = CreateGadgetState(gadgetData, AllGadgetStateData[i]);
        }

        return result;
    }

    private GadgetState CreateGadgetState(
        GadgetData gadgetData,
        GadgetStateArchetypeData gadgetStateArchetypeData)
    {
        var hitBox = CreateHitBoxForState(gadgetData, gadgetStateArchetypeData);

        return new GadgetState(
            GadgetBehaviour,
            gadgetStateArchetypeData.OnLemmingEnterActions,
            gadgetStateArchetypeData.OnLemmingPresentActions,
            gadgetStateArchetypeData.OnLemmingExitActions,
            null,
            null,
            hitBox);
    }

    private HitBox CreateHitBoxForState(
        GadgetData gadgetData,
        GadgetStateArchetypeData gadgetStateArchetypeData)
    {
        var hitBoxRegion = CreateHitBoxLevelRegion(gadgetData, gadgetStateArchetypeData);

        var lemmingFilters = new List<ILemmingFilter>();

        if (gadgetStateArchetypeData.AllowedActions is not null)
        {
            var actionFilter = new LemmingActionFilter();
            actionFilter.RegisterActions(gadgetStateArchetypeData.AllowedActions);
            lemmingFilters.Add(actionFilter);
        }

        if (gadgetStateArchetypeData.AllowedStates is not null)
        {
            var states = gadgetStateArchetypeData.AllowedStates.ToArray();
            var stateFilter = new LemmingStateFilter(states);
            lemmingFilters.Add(stateFilter);
        }

        if (gadgetStateArchetypeData.AllowedOrientations is not null)
        {
            var orientationFilter = new LemmingOrientationFilter();
            var gadgetOrientationId = gadgetData.Orientation.RotNum;

            foreach (var orientation in gadgetStateArchetypeData.AllowedOrientations)
            {
                var rotatedOrientationId = (orientation.RotNum + gadgetOrientationId) & 3;
                orientationFilter.RegisterOrientation(Orientation.AllItems[rotatedOrientationId]);
            }

            lemmingFilters.Add(orientationFilter);
        }

        if (gadgetStateArchetypeData.AllowedFacingDirection is not null)
        {
            var facingDirectionFilter = new LemmingFacingDirectionFilter(gadgetStateArchetypeData.AllowedFacingDirection);
            lemmingFilters.Add(facingDirectionFilter);
        }

        if (gadgetData.TryGetProperty(GadgetProperty.TeamId, out var teamId))
        {
            var team = Team.AllItems[teamId];
            var teamFilter = new LemmingTeamFilter(team);
            lemmingFilters.Add(teamFilter);
        }

        var allFilters = lemmingFilters.Count > 0
            ? lemmingFilters.ToArray()
            : Array.Empty<ILemmingFilter>();

        var hitBox = new HitBox(
            hitBoxRegion,
            allFilters);

        return hitBox;
    }

    private ILevelRegion CreateHitBoxLevelRegion(
        GadgetData gadgetData,
        GadgetStateArchetypeData gadgetStateArchetypeData)
    {
        var nullableRectangularTriggerData = gadgetStateArchetypeData.TriggerData;
        if (!nullableRectangularTriggerData.HasValue)
            return EmptyLevelRegion.Instance;

        return CreateRectangularHitBoxLevelRegion(nullableRectangularTriggerData.Value);

        RectangularLevelRegion CreateRectangularHitBoxLevelRegion(RectangularTriggerData triggerData)
        {
            gadgetData.GetDihedralTransformation(out var dihedralTransformation);

            dihedralTransformation.Transform(
                triggerData.TriggerX,
                triggerData.TriggerY,
                SpriteData.SpriteWidth,
                SpriteData.SpriteHeight,
                out var tX,
                out var tY);

            var p0 = new LevelPosition(tX, tY);

            dihedralTransformation.Transform(
                triggerData.TriggerX + triggerData.TriggerWidth - 1,
                triggerData.TriggerY + triggerData.TriggerHeight - 1,
                SpriteData.SpriteWidth,
                SpriteData.SpriteHeight,
                out tX,
                out tY);

            var p1 = new LevelPosition(tX, tY);

            return new RectangularLevelRegion(p0, p1);
        }
    }
}