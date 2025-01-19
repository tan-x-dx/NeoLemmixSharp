﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class HitBoxGadgetBuilder : IGadgetBuilder
{
    public required ResizeType ResizeType { get; init; }
    public required int GadgetBuilderId { get; init; }
    public required GadgetStateArchetypeDataAaa[] AllGadgetStateData { get; init; }

    public required SpriteData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        LemmingManager lemmingManager)
    {
        var gadgetBounds = GetGadgetBounds(gadgetData);
        var gadgetStates = GetGadgetStates(gadgetData, gadgetBounds);
        var initialStateIndex = gadgetData.InitialStateId;

        var lemmingTracker = new LemmingTracker(lemmingManager);

        return new HitBoxGadget(
            gadgetData.Id,
            gadgetData.Orientation,
            gadgetBounds,
            lemmingTracker,
            gadgetStates,
            initialStateIndex,
            ResizeType);
    }

    private GadgetBounds GetGadgetBounds(GadgetData gadgetData)
    {
        var result = new GadgetBounds
        {
            X = gadgetData.X,
            Y = gadgetData.Y
        };

        result.Width = ResizeType.HasFlag(ResizeType.ResizeHorizontal)
            ? gadgetData.GetProperty(GadgetProperty.Width)
            : SpriteData.SpriteWidth;

        result.Height = ResizeType.HasFlag(ResizeType.ResizeVertical)
            ? gadgetData.GetProperty(GadgetProperty.Height)
            : SpriteData.SpriteHeight;

        return result;
    }

    private GadgetState[] GetGadgetStates(
        GadgetData gadgetData,
        GadgetBounds hitBoxGadgetBounds)
    {
        var result = new GadgetState[AllGadgetStateData.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var gadgetStateArchetypeData = AllGadgetStateData[i];

            var animationController = gadgetStateArchetypeData.GetAnimationController();
            var hitBoxRegion = CreateHitBoxRegion(
                hitBoxGadgetBounds,
                gadgetStateArchetypeData.RegionData);
            var hitBoxFilters = CreateHitBoxFilters(
                gadgetData,
                gadgetStateArchetypeData);

            result[i] = new GadgetState(
                animationController,
                hitBoxRegion,
                hitBoxFilters);
        }

        return result;
    }

    private static LemmingHitBoxFilter[] CreateHitBoxFilters(
        GadgetData gadgetData,
        GadgetStateArchetypeDataAaa gadgetStateArchetypeData)
    {
        var result = new LemmingHitBoxFilter[gadgetStateArchetypeData.HitBoxData.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var hitBoxData = gadgetStateArchetypeData.HitBoxData[i];
            var solidityType = hitBoxData.SolidityType;
            var hitBoxBehaviour = hitBoxData.HitBoxBehaviour;
            var criteria = GetLemmingCriteria(gadgetData, hitBoxData);

            result[i] = new LemmingHitBoxFilter(
                solidityType,
                hitBoxBehaviour,
                criteria,
                hitBoxData.OnLemmingEnterActions,
                hitBoxData.OnLemmingPresentActions,
                hitBoxData.OnLemmingExitActions);
        }

        return result;
    }

    private static ILemmingCriterion[] GetLemmingCriteria(
        GadgetData gadgetData,
        HitBoxData hitBoxData)
    {
        var numberOfCriteria =
            Helpers.CountIfNotNull(hitBoxData.AllowedActions) +
            Helpers.CountIfNotNull(hitBoxData.AllowedStates) +
            Helpers.CountIfNotNull(hitBoxData.AllowedOrientations) +
            Helpers.CountIfNotNull(hitBoxData.AllowedFacingDirection) +
            (gadgetData.HasProperty(GadgetProperty.TeamId) ? 1 : 0);
        if (numberOfCriteria == 0)
            return [];

        var result = new ILemmingCriterion[numberOfCriteria];

        numberOfCriteria = 0;
        if (hitBoxData.AllowedActions is not null)
        {
            var lemmingActionCriterion = new LemmingActionCriterion();
            lemmingActionCriterion.RegisterActions(hitBoxData.AllowedActions);
            result[numberOfCriteria++] = lemmingActionCriterion;
        }

        if (hitBoxData.AllowedStates is not null)
        {
            var states = hitBoxData.AllowedStates.ToArray();
            var stateFilter = new LemmingStateCriterion(states);
            result[numberOfCriteria++] = stateFilter;
        }

        if (hitBoxData.AllowedOrientations is not null)
        {
            var orientationFilter = new LemmingOrientationFilter();
            var gadgetOrientationId = gadgetData.Orientation.RotNum;

            foreach (var orientation in hitBoxData.AllowedOrientations)
            {
                var rotatedOrientationId = (orientation.RotNum + gadgetOrientationId) & 3;
                orientationFilter.RegisterOrientation(Orientation.AllItems[rotatedOrientationId]);
            }

            result[numberOfCriteria++] = orientationFilter;
        }

        if (hitBoxData.AllowedFacingDirection is not null)
        {
            var facingDirectionFilter = new LemmingFacingDirectionCriterion(hitBoxData.AllowedFacingDirection);
            result[numberOfCriteria++] = facingDirectionFilter;
        }

        if (gadgetData.TryGetProperty(GadgetProperty.TeamId, out var teamId))
        {
            var team = Team.AllItems[teamId];
            var teamFilter = new LemmingTeamCriterion(team);
            result[numberOfCriteria++] = teamFilter;
        }

        return result;
    }

    private static IHitBoxRegion CreateHitBoxRegion(
        GadgetBounds hitBoxGadgetBounds,
        HitBoxRegionData hitBoxRegionData)
    {
        return hitBoxRegionData.HitBoxType switch
        {
            HitBoxType.Empty => EmptyHitBoxRegion.Instance,
            HitBoxType.ResizableRectangular => CreateResizableRectangularHitBoxRegion(hitBoxGadgetBounds, hitBoxRegionData.HitBoxData),
            HitBoxType.Rectangular => CreateRectangularHitBoxRegion(hitBoxRegionData.HitBoxData),
            HitBoxType.PointSet => new PointSetHitBoxRegion(hitBoxRegionData.HitBoxData),

            _ => throw new ArgumentOutOfRangeException(nameof(hitBoxRegionData.HitBoxType), hitBoxRegionData.HitBoxType, "Unknown HitBoxType")
        };

        static RectangularHitBoxRegion CreateRectangularHitBoxRegion(
            ReadOnlySpan<LevelPosition> hitBoxRegionData)
        {
            if (hitBoxRegionData.Length != 2)
                throw new InvalidOperationException("Expected data of length 2");

            var p0 = hitBoxRegionData[0];
            var p1 = hitBoxRegionData[1];
            return new RectangularHitBoxRegion(p0.X, p0.Y, p1.X, p1.Y);
        }

        static ResizableRectangularHitBoxRegion CreateResizableRectangularHitBoxRegion(
            GadgetBounds hitBoxGadgetBounds,
            ReadOnlySpan<LevelPosition> hitBoxRegionData)
        {
            if (hitBoxRegionData.Length != 2)
                throw new InvalidOperationException("Expected data of length 2");

            var p0 = hitBoxRegionData[0];
            var p1 = hitBoxRegionData[1];
            return new ResizableRectangularHitBoxRegion(hitBoxGadgetBounds, p0.X, p0.Y, p1.X, p1.Y);
        }
    }
}


/*
private IHitBoxRegion CreateHitBoxLevelRegion(
    GadgetData gadgetData,
    HitBoxType triggerType,
    ReadOnlySpan<LevelPosition> triggerData)
{
    if (triggerType == HitBoxType.Rectangular)
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




private RectangularHitBoxRegion CreateRectangularHitBoxLevelRegion(
    GadgetData gadgetData,
    ReadOnlySpan<LevelPosition> triggerData,
    RectangularHitBoxRegion gadgetBounds)
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

    return new RectangularHitBoxRegion(p0.X, p0.Y, p1.X, p1.Y);
}
*/