using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OrientationToHitBoxRegionLookup = NeoLemmixSharp.Common.Util.Collections.BitArrays.BitArrayDictionary<NeoLemmixSharp.Common.Orientation.OrientationHasher, NeoLemmixSharp.Common.Util.Collections.BitArrays.BitBuffer32, NeoLemmixSharp.Common.Orientation, NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes.IHitBoxRegion>;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class HitBoxGadgetArchetypeBuilder : IGadgetArchetypeBuilder
{
    public required string StyleName { get; init; }
    public required string PieceName { get; init; }

    public required ResizeType ResizeType { get; init; }
    public required GadgetStateArchetypeData[] AllGadgetStateData { get; init; }

    public required SpriteData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TeamManager teamManager)
    {
        var currentGadgetBounds = GetGadgetBounds(gadgetData);
        var previousGadgetBounds = new GadgetBounds(currentGadgetBounds);
        var resizeType = GetResizeTypeForGadgetOrientation(gadgetData);
        var gadgetStates = GetGadgetStates(gadgetData, currentGadgetBounds, teamManager);
        var initialStateIndex = gadgetData.InitialStateId;

        var lemmingTracker = new LemmingTracker(lemmingManager);

        bool isFastForward;
        if (gadgetData.TryGetProperty(GadgetProperty.IsFastForwards, out var fastForwardValue))
        {
            isFastForward = fastForwardValue != 0;
        }
        else
        {
            isFastForward = false;
        }

        return new HitBoxGadget(
            resizeType,
            lemmingTracker,
            gadgetStates,
            initialStateIndex)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = currentGadgetBounds,
            PreviousGadgetBounds = previousGadgetBounds,

            IsFastForward = isFastForward
        };
    }

    private GadgetBounds GetGadgetBounds(GadgetData gadgetData)
    {
        var result = new GadgetBounds
        {
            Position = gadgetData.Position
        };

        var size = new Size(
            ResizeType.CanResizeHorizontally() ? gadgetData.GetProperty(GadgetProperty.Width) : SpriteData.SpriteSize.W,
            ResizeType.CanResizeVertically() ? gadgetData.GetProperty(GadgetProperty.Height) : SpriteData.SpriteSize.H);

        size = new DihedralTransformation(gadgetData.Orientation, gadgetData.FacingDirection).Transform(size);

        result.Width = size.W;
        result.Height = size.H;

        return result;
    }

    private ResizeType GetResizeTypeForGadgetOrientation(GadgetData gadgetData)
    {
        return new DihedralTransformation(gadgetData.Orientation, gadgetData.FacingDirection).Transform(ResizeType);
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

            //   var animationController = gadgetStateArchetypeData.GetAnimationController();
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
                null!);
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
            hitBoxData.AllowedActions.CountIfNotNull() +
            hitBoxData.AllowedStates.CountIfNotNull() +
            hitBoxData.AllowedOrientations.CountIfNotNull() +
            hitBoxData.AllowedFacingDirection.CountIfNotNull() +
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
            result[numberOfCriteria++] = new LemmingStateCriterion(hitBoxData.AllowedStates);
        }

        if (hitBoxData.AllowedOrientations is not null)
        {
            var orientationFilter = new LemmingOrientationFilter();
            var gadgetRotNum = gadgetData.Orientation.RotNum;

            foreach (var orientation in hitBoxData.AllowedOrientations)
            {
                var rotatedOrientation = new Orientation(orientation.RotNum + gadgetRotNum);
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
            ReadOnlySpan<Point> hitBoxRegionData)
        {
            if (hitBoxRegionData.Length != 2)
                throw new InvalidOperationException("Expected exactly two points of data");

            var dihedralTransformation = new DihedralTransformation(gadgetData.Orientation, gadgetData.FacingDirection);

            var p0 = dihedralTransformation.Transform(hitBoxRegionData[0], SpriteData.SpriteSize);
            var p1 = dihedralTransformation.Transform(hitBoxRegionData[1], SpriteData.SpriteSize);

            return new RectangularHitBoxRegion(p0, p1);
        }

        [SkipLocalsInit]
        PointSetHitBoxRegion CreatePointSetHitBoxRegion(
            GadgetData gadgetData,
            ReadOnlySpan<Point> triggerData)
        {
            var dihedralTransformation = new DihedralTransformation(gadgetData.Orientation, gadgetData.FacingDirection);

            Span<Point> adjustedPoints = triggerData.Length > 32
                ? new Point[triggerData.Length]
                : stackalloc Point[triggerData.Length];

            for (var i = 0; i < triggerData.Length; i++)
            {
                adjustedPoints[i] = dihedralTransformation.Transform(triggerData[i], SpriteData.SpriteSize);
            }

            return new PointSetHitBoxRegion(adjustedPoints);
        }

        static ResizableRectangularHitBoxRegion CreateResizableRectangularHitBoxRegion(
            GadgetBounds hitBoxGadgetBounds,
            ReadOnlySpan<Point> hitBoxRegionData)
        {
            if (hitBoxRegionData.Length != 2)
                throw new InvalidOperationException("Expected data of length 2");

            var p0 = hitBoxRegionData[0];
            var p1 = hitBoxRegionData[1];
            return new ResizableRectangularHitBoxRegion(hitBoxGadgetBounds, p0.X, p0.Y, p1.X, p1.Y);
        }
    }
}