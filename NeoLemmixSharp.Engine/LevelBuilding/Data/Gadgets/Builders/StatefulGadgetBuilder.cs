using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class StatefulGadgetBuilder : IGadgetAnimationData
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

        foreach (var gadgetStateArchetypeData in AllGadgetStateData)
        {
            gadgetStateArchetypeData.Clear();
        }

        return result;
    }

    public IEnumerable<GadgetAnimationArchetypeData> AnimationArchetypes()
    {
        throw new NotImplementedException();
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

        var animationController = gadgetStateArchetypeData.GetAnimationController();

        return new GadgetState(
            GadgetBehaviour,
            gadgetStateArchetypeData.OnLemmingEnterActions,
            gadgetStateArchetypeData.OnLemmingPresentActions,
            gadgetStateArchetypeData.OnLemmingExitActions,
            animationController,
            hitBox);
    }

    private HitBox CreateHitBoxForState(
        GadgetData gadgetData,
        GadgetStateArchetypeData gadgetStateArchetypeData)
    {
        var triggerData = gadgetStateArchetypeData.TriggerData;
        if (triggerData.Length == 0)
            return HitBox.Empty;

        var hitBoxRegion = CreateHitBoxLevelRegion(gadgetData, gadgetStateArchetypeData.TriggerType, triggerData);

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

        var hitBox = new HitBox(
            hitBoxRegion,
            lemmingFilters.ToArray());

        return hitBox;
    }

    private ILevelRegion CreateHitBoxLevelRegion(
        GadgetData gadgetData,
        TriggerType triggerType,
        LevelPosition[] triggerData)
    {
        if (triggerType == TriggerType.Rectangular)
            return CreateRectangularHitBoxLevelRegion(gadgetData, triggerData);

        return CreatePointSetHitBoxLevelRegion(gadgetData, triggerData);
    }

    private RectangularLevelRegion CreateRectangularHitBoxLevelRegion(GadgetData gadgetData, LevelPosition[] triggerData)
    {
        if (triggerData.Length != 2)
            throw new InvalidOperationException("Expected exactly two points of data");

        gadgetData.GetDihedralTransformation(out var dihedralTransformation);

        var p0 = triggerData[0];
        var p1 = triggerData[1];

        dihedralTransformation.Transform(
            p0.X,
            p0.Y,
            SpriteData.SpriteWidth,
            SpriteData.SpriteHeight,
            out var tX,
            out var tY);

        p0 = new LevelPosition(tX, tY);

        dihedralTransformation.Transform(
            p1.X,
            p1.Y,
            SpriteData.SpriteWidth,
            SpriteData.SpriteHeight,
            out tX,
            out tY);

        p1 = new LevelPosition(tX, tY);

        return new RectangularLevelRegion(p0, p1);
    }

    [SkipLocalsInit]
    private PointSetLevelRegion CreatePointSetHitBoxLevelRegion(GadgetData gadgetData, LevelPosition[] triggerData)
    {
        gadgetData.GetDihedralTransformation(out var dihedralTransformation);

        Span<LevelPosition> adjustedPoints = triggerData.Length > 64
            ? new LevelPosition[triggerData.Length]
            : stackalloc LevelPosition[triggerData.Length];

        for (var i = 0; i < triggerData.Length; i++)
        {
            var originalPoint = triggerData[i];
            ref var transformedPoint = ref adjustedPoints[i];
            dihedralTransformation.Transform(
                originalPoint.X,
                originalPoint.Y,
                SpriteData.SpriteWidth,
                SpriteData.SpriteHeight,
                out var tX,
                out var tY);

            transformedPoint = new LevelPosition(tX, tY);
        }

        return new PointSetLevelRegion(adjustedPoints);
    }
}