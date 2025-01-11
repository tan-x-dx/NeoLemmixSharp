﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class StatefulGadgetBuilder : IGadgetBuilder
{
    public required int GadgetBuilderId { get; init; }
    public required GadgetStateArchetypeData[] AllGadgetStateData { get; init; }

    public required SpriteData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        LemmingManager lemmingManager)
    {
        var bounds = new RectangularHitBoxRegion(
            gadgetData.X,
            gadgetData.Y,
            SpriteData.SpriteWidth,
            SpriteData.SpriteHeight);

        var gadgetStates = CreateStates(gadgetData);
        var gadgetRenderer = gadgetSpriteBuilder.BuildStatefulGadgetRenderer(this, gadgetData);
        var itemTracker = new LemmingTracker(lemmingManager);

        var result = new HitBoxGadget(
            gadgetData.Id,
            gadgetData.Orientation,
            bounds,
            gadgetRenderer,
            gadgetStates,
            itemTracker);

        result.SetNextState(gadgetData.InitialStateId);

        gadgetRenderer?.SetGadget(result);

        foreach (var gadgetStateArchetypeData in AllGadgetStateData)
        {
            gadgetStateArchetypeData.Clear();
        }

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
        var hitBoxRegion = CreateHitBoxRegionForState(gadgetData, gadgetStateArchetypeData);

        var animationController = gadgetStateArchetypeData.GetAnimationController();

        return new GadgetState(
            gadgetStateArchetypeData.OnLemmingEnterActions,
            gadgetStateArchetypeData.OnLemmingPresentActions,
            gadgetStateArchetypeData.OnLemmingExitActions,
            animationController,
            hitBoxRegion);
    }

    private IHitBoxRegion CreateHitBoxRegionForState(
        GadgetData gadgetData,
        GadgetStateArchetypeData gadgetStateArchetypeData)
    {
        var triggerData = gadgetStateArchetypeData.TriggerData;
        if (triggerData.Length == 0)
            return new EmptyHitBoxRegion(new LevelPosition(gadgetData.X, gadgetData.Y));

        var hitBoxRegion = CreateHitBoxLevelRegion(gadgetData, gadgetStateArchetypeData.TriggerType, triggerData);

        var lemmingFilters = new List<ILemmingCriterion>();

        if (gadgetStateArchetypeData.AllowedActions is not null)
        {
            var actionFilter = new LemmingActionCriterion();
            actionFilter.RegisterActions(gadgetStateArchetypeData.AllowedActions);
            lemmingFilters.Add(actionFilter);
        }

        if (gadgetStateArchetypeData.AllowedStates is not null)
        {
            var states = gadgetStateArchetypeData.AllowedStates.ToArray();
            var stateFilter = new LemmingStateCriterion(states);
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
            var facingDirectionFilter = new LemmingFacingDirectionCriterion(gadgetStateArchetypeData.AllowedFacingDirection);
            lemmingFilters.Add(facingDirectionFilter);
        }

        if (gadgetData.TryGetProperty(GadgetProperty.TeamId, out var teamId))
        {
            var team = Team.AllItems[teamId];
            var teamFilter = new LemmingTeamCriterion(team);
            lemmingFilters.Add(teamFilter);
        }

        var hitBox = new HitBox(
            hitBoxRegion,
            lemmingFilters.ToArray());

        return hitBox;
    }

    private IHitBoxRegion CreateHitBoxLevelRegion(
        GadgetData gadgetData,
        TriggerType triggerType,
        ReadOnlySpan<LevelPosition> triggerData)
    {
        if (triggerType == TriggerType.Rectangular)
            return CreateRectangularHitBoxLevelRegion(gadgetData, triggerData);

        return CreatePointSetHitBoxLevelRegion(gadgetData, triggerData);
    }

    private RectangularHitBoxRegion CreateRectangularHitBoxLevelRegion(
        GadgetData gadgetData,
        ReadOnlySpan<LevelPosition> triggerData)
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

        return new RectangularHitBoxRegion(p0, p1);
    }

    [SkipLocalsInit]
    private PointSetHitBoxRegion CreatePointSetHitBoxLevelRegion(
        GadgetData gadgetData,
        ReadOnlySpan<LevelPosition> triggerData)
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

        return new PointSetHitBoxRegion(adjustedPoints);
    }
}