using NeoLemmixSharp.Common.Util;
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
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class HitBoxGadgetArchetypeBuilder : IGadgetArchetypeBuilder
{
    public required int GadgetBuilderId { get; init; }
    public required string Style { get; init; }
    public required string GadgetPiece { get; init; }

    public required ResizeType ResizeType { get; init; }
    public required GadgetStateArchetypeData[] AllGadgetStateData { get; init; }

    public required SpriteData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TeamManager teamManager)
    {
        var gadgetBounds = GetGadgetBounds(gadgetData);
        var resizeType = GetResizeTypeForGadgetOrientation(gadgetData);
        var gadgetStates = GetGadgetStates(gadgetData, gadgetBounds, teamManager);
        var initialStateIndex = gadgetData.InitialStateId;

        var lemmingTracker = new LemmingTracker(lemmingManager);

        return new HitBoxGadget(
            gadgetData.Id,
            gadgetData.Orientation,
            gadgetBounds,
            resizeType,
            lemmingTracker,
            gadgetStates,
            initialStateIndex);
    }

    private GadgetBounds GetGadgetBounds(GadgetData gadgetData)
    {
        var result = new GadgetBounds
        {
            X = gadgetData.X,
            Y = gadgetData.Y
        };

        if (gadgetData.Orientation.IsParallelTo(Orientation.Down))
        {
            result.Width = ResizeType.CanResizeHorizontally()
                ? gadgetData.GetProperty(GadgetProperty.Width)
                : SpriteData.SpriteWidth;
            result.Height = ResizeType.CanResizeVertically()
                ? gadgetData.GetProperty(GadgetProperty.Height)
                : SpriteData.SpriteHeight;
        }
        else
        {
            result.Width = ResizeType.CanResizeVertically()
                ? gadgetData.GetProperty(GadgetProperty.Height)
                : SpriteData.SpriteHeight;
            result.Height = ResizeType.CanResizeHorizontally()
                ? gadgetData.GetProperty(GadgetProperty.Width)
                : SpriteData.SpriteWidth;
        }

        return result;
    }

    private ResizeType GetResizeTypeForGadgetOrientation(GadgetData gadgetData)
    {
        if (gadgetData.Orientation.IsParallelTo(Orientation.Down))
            return ResizeType;

        return ResizeType.SwapComponents();
    }

    private GadgetState[] GetGadgetStates(
        GadgetData gadgetData,
        GadgetBounds hitBoxGadgetBounds,
        TeamManager teamManager)
    {
        var result = new GadgetState[AllGadgetStateData.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var gadgetStateArchetypeData = AllGadgetStateData[i];

            var animationController = gadgetStateArchetypeData.GetAnimationController();
            var hitBoxRegionLookup = CreateHitBoxRegionLookup(
                gadgetData,
                hitBoxGadgetBounds,
                gadgetStateArchetypeData.RegionData);
            var hitBoxFilters = CreateHitBoxFilters(
                gadgetData,
                gadgetStateArchetypeData,
                teamManager);

            result[i] = new GadgetState(
                hitBoxFilters,
                hitBoxRegionLookup,
                animationController);
        }

        return result;
    }

    private static LemmingHitBoxFilter[] CreateHitBoxFilters(
        GadgetData gadgetData,
        GadgetStateArchetypeData gadgetStateArchetypeData,
        TeamManager teamManager)
    {
        var result = new LemmingHitBoxFilter[gadgetStateArchetypeData.HitBoxData.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var hitBoxData = gadgetStateArchetypeData.HitBoxData[i];
            var solidityType = hitBoxData.SolidityType;
            var hitBoxBehaviour = hitBoxData.HitBoxBehaviour;
            var criteria = GetLemmingCriteria(gadgetData, hitBoxData, teamManager);

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
        HitBoxData hitBoxData,
        TeamManager teamManager)
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
                var rotatedOrientation = new Orientation(orientation.RotNum + gadgetOrientationId);
                orientationFilter.RegisterOrientation(rotatedOrientation);
            }

            result[numberOfCriteria++] = orientationFilter;
        }

        if (hitBoxData.AllowedFacingDirection.HasValue)
        {
            var facingDirectionFilter = LemmingFacingDirectionCriterion.ForFacingDirection(hitBoxData.AllowedFacingDirection.Value);
            result[numberOfCriteria++] = facingDirectionFilter;
        }

        if (gadgetData.TryGetProperty(GadgetProperty.TeamId, out var teamId))
        {
            var team = teamManager.AllItems[teamId];
            var teamFilter = new LemmingTeamCriterion(team);
            result[numberOfCriteria++] = teamFilter;
        }

        Debug.Assert(numberOfCriteria == result.Length);

        return result;
    }

    private OrientationToHitBoxRegionLookup CreateHitBoxRegionLookup(
        GadgetData gadgetData,
        GadgetBounds hitBoxGadgetBounds,
        ReadOnlySpan<HitBoxRegionData> hitBoxRegionData)
    {
        var result = Orientation.CreateBitArrayDictionary<IHitBoxRegion>();

        foreach (var item in hitBoxRegionData)
        {
            // Can skip this case since it doesn't do anything interesting anyway
            if (item.HitBoxType == HitBoxType.Empty)
                continue;

            IHitBoxRegion hitBoxRegion = item.HitBoxType switch
            {
                HitBoxType.Empty => EmptyHitBoxRegion.Instance,
                HitBoxType.ResizableRectangular => CreateResizableRectangularHitBoxRegion(hitBoxGadgetBounds, item.HitBoxDefinitionData),
                HitBoxType.Rectangular => CreateRectangularHitBoxRegion(gadgetData, item.HitBoxDefinitionData),
                HitBoxType.PointSet => CreatePointSetHitBoxRegion(gadgetData, item.HitBoxDefinitionData),

                _ => throw new ArgumentOutOfRangeException(nameof(item.HitBoxType), item.HitBoxType, "Unknown HitBoxType")
            };

            result.Add(item.Orientation, hitBoxRegion);
        }

        return result;

        RectangularHitBoxRegion CreateRectangularHitBoxRegion(
            GadgetData gadgetData,
            ReadOnlySpan<LevelPosition> hitBoxRegionData)
        {
            if (hitBoxRegionData.Length != 2)
                throw new InvalidOperationException("Expected exactly two points of data");

            gadgetData.GetDihedralTransformation(out var dihedralTransformation);

            var p0 = hitBoxRegionData[0];
            var p1 = hitBoxRegionData[1];

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
        PointSetHitBoxRegion CreatePointSetHitBoxRegion(
            GadgetData gadgetData,
            ReadOnlySpan<LevelPosition> triggerData)
        {
            gadgetData.GetDihedralTransformation(out var dihedralTransformation);

            Span<LevelPosition> adjustedPoints = triggerData.Length > 32
                ? new LevelPosition[triggerData.Length]
                : stackalloc LevelPosition[triggerData.Length];

            for (var i = 0; i < triggerData.Length; i++)
            {
                var originalPoint = triggerData[i];
                dihedralTransformation.Transform(
                    originalPoint.X,
                    originalPoint.Y,
                    SpriteData.SpriteWidth,
                    SpriteData.SpriteHeight,
                    out var tX,
                    out var tY);

                adjustedPoints[i] = new LevelPosition(tX, tY);
            }

            return new PointSetHitBoxRegion(adjustedPoints);
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