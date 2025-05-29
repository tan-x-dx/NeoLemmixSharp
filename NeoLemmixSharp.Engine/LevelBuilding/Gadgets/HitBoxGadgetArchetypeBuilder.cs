using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using OrientationToHitBoxRegionLookup = NeoLemmixSharp.Common.Util.Collections.BitArrays.BitArrayDictionary<NeoLemmixSharp.Common.Orientation.OrientationHasher, NeoLemmixSharp.Common.Util.Collections.BitArrays.BitBuffer32, NeoLemmixSharp.Common.Orientation, NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes.IHitBoxRegion>;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public sealed class HitBoxGadgetArchetypeBuilder : IGadgetArchetypeBuilder
{
    public required StyleIdentifier StyleName { get; init; }
    public required PieceIdentifier PieceName { get; init; }

    public required ResizeType ResizeType { get; init; }
    public required GadgetStateArchetypeData[] AllGadgetStateData { get; init; }

    public required SpriteArchetypeData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetRendererBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        var currentGadgetBounds = GetGadgetBounds(gadgetData);
        var resizeType = GetResizeTypeForGadgetOrientation(gadgetData);
        var gadgetStates = GetGadgetStates(gadgetData, currentGadgetBounds, tribeManager);
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

        return null;

        /*
        return new HitBoxGadget(
            resizeType,
            lemmingTracker,
            gadgetStates,
            initialStateIndex)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = currentGadgetBounds,

            IsFastForward = isFastForward
        };*/
    }

    private GadgetBounds GetGadgetBounds(GadgetData gadgetData)
    {
        var result = new GadgetBounds
        {
            Position = gadgetData.Position
        };

        var size = new Size(
            ResizeType.CanResizeHorizontally() ? gadgetData.GetProperty(GadgetProperty.Width) : SpriteData.BaseSpriteSize.W,
            ResizeType.CanResizeVertically() ? gadgetData.GetProperty(GadgetProperty.Height) : SpriteData.BaseSpriteSize.H);

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
        GadgetBounds currentGadgetBounds,
        TribeManager tribeManager)
    {
        var result = new GadgetState[AllGadgetStateData.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var gadgetStateArchetypeData = AllGadgetStateData[i];

            //   var animationController = gadgetStateArchetypeData.GetAnimationController();
            var hitBoxRegionLookup = CreateHitBoxRegionLookup(
                gadgetData,
                currentGadgetBounds,
                gadgetStateArchetypeData.RegionData);
            var hitBoxFilters = CreateHitBoxFilters(
                gadgetData,
                gadgetStateArchetypeData,
                tribeManager);
            //var animationController = SpriteData.CreateAnimationController(i, currentGadgetBounds);

            result[i] = new GadgetState(
                gadgetStateArchetypeData.StateName,
                hitBoxFilters,
                hitBoxRegionLookup,
                null);
        }

        return result;
    }

    private static LemmingHitBoxFilter[] CreateHitBoxFilters(
        GadgetData gadgetData,
        GadgetStateArchetypeData gadgetStateArchetypeData,
        TribeManager tribeManager)
    {
        var result = new LemmingHitBoxFilter[gadgetStateArchetypeData.HitBoxData.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var hitBoxData = gadgetStateArchetypeData.HitBoxData[i];
            var solidityType = hitBoxData.SolidityType;
            var hitBoxBehaviour = hitBoxData.HitBoxBehaviour;
            var criteria = GetLemmingCriteria(gadgetData, hitBoxData, tribeManager);

            GadgetActionBuilder.ReadGadgetActions(
                hitBoxData,
                out var onLemmingEnterActions,
                out var onLemmingPresentActions,
                out var onLemmingExitActions);

            result[i] = new LemmingHitBoxFilter(
                solidityType,
                hitBoxBehaviour,
                criteria,
                onLemmingEnterActions,
                onLemmingPresentActions,
                onLemmingExitActions);
        }

        return result;
    }

    private static LemmingCriterion[] GetLemmingCriteria(
        GadgetData gadgetData,
        HitBoxData hitBoxData,
        TribeManager tribeManager)
    {
        var numberOfCriteria = CalculateNumberOfCriteria(gadgetData, hitBoxData);
        var result = CollectionsHelper.GetArrayForSize<LemmingCriterion>(numberOfCriteria);

        var i = 0;

        if (hitBoxData.AllowedLemmingOrientationIds != 0)
            result[i++] = CreateOrientationCriterion(gadgetData, hitBoxData);

        if (hitBoxData.AllowedFacingDirectionId != 0)
            result[i++] = CreateFacingDirectionCriterion(hitBoxData);

        if (hitBoxData.AllowedLemmingActionIds.Length > 0)
            result[i++] = CreateLemmingActionCriterion(hitBoxData);

        if (hitBoxData.AllowedLemmingStateIds.Length > 0)
            result[i++] = CreateLemmingStateCriterion(hitBoxData);

        if (gadgetData.TryGetProperty(GadgetProperty.TribeId, out var tribeId))
            result[i++] = CreateTribeCriterion(tribeManager, tribeId);

        Debug.Assert(i == result.Length);

        Array.Sort(result);

        return result;

        static int CalculateNumberOfCriteria(GadgetData gadgetData, HitBoxData hitBoxData)
        {
            return (hitBoxData.AllowedLemmingOrientationIds != 0 ? 1 : 0) +
                   (hitBoxData.AllowedFacingDirectionId != 0 ? 1 : 0) +
                   (hitBoxData.AllowedLemmingActionIds.Length > 0 ? 1 : 0) +
                   (hitBoxData.AllowedLemmingStateIds.Length > 0 ? 1 : 0) +
                   (gadgetData.HasProperty(GadgetProperty.TribeId) ? 1 : 0);
        }

        static LemmingActionCriterion CreateLemmingActionCriterion(
            HitBoxData hitBoxData)
        {
            var lemmingActionSet = LemmingAction.CreateBitArraySet();
            lemmingActionSet.ReadFrom(hitBoxData.AllowedLemmingActionIds);
            var lemmingActionCriterion = new LemmingActionCriterion(lemmingActionSet);
            return lemmingActionCriterion;
        }

        static LemmingStateCriterion CreateLemmingStateCriterion(
            HitBoxData hitBoxData)
        {
            var stateChangerSet = ILemmingStateChanger.CreateBitArraySet();
            stateChangerSet.ReadFrom(hitBoxData.AllowedLemmingStateIds);
            var lemmingStateCriteria = new LemmingStateCriterion(stateChangerSet);
            return lemmingStateCriteria;
        }

        [SkipLocalsInit]
        static LemmingOrientationCriterion CreateOrientationCriterion(
           GadgetData gadgetData,
           HitBoxData hitBoxData)
        {
            const int OrientationBitMask = (1 << EngineConstants.NumberOfOrientations) - 1;

            var gadgetRotNum = gadgetData.Orientation.RotNum;

            uint orientationData = hitBoxData.AllowedLemmingOrientationIds;
            orientationData &= OrientationBitMask;
            var orientationSet = Orientation.CreateBitArraySet();
            orientationSet.ReadFrom(new ReadOnlySpan<uint>(ref orientationData));

            Span<Orientation> tempRotations = stackalloc Orientation[orientationSet.Count];
            var i = 0;

            foreach (var orientation in orientationSet)
            {
                var rotatedOrientation = new Orientation(orientation.RotNum + gadgetRotNum);
                tempRotations[i++] = rotatedOrientation;
            }

            orientationSet.Clear();
            foreach (var rotatedOrientation in tempRotations)
            {
                orientationSet.Add(rotatedOrientation);
            }

            return new LemmingOrientationCriterion(orientationSet);
        }

        static LemmingFacingDirectionCriterion CreateFacingDirectionCriterion(
            HitBoxData hitBoxData)
        {
            var facingDirectionCriterion = LemmingFacingDirectionCriterion.ForFacingDirection(hitBoxData.AllowedFacingDirectionId);
            return facingDirectionCriterion;
        }

        static LemmingTribeCriterion CreateTribeCriterion(
            TribeManager tribeManager,
            int tribeId)
        {
            var tribe = tribeManager.AllItems[tribeId];
            var tribeFilter = new LemmingTribeCriterion(tribe);
            return tribeFilter;
        }
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

                _ => Helpers.ThrowUnknownEnumValueException<HitBoxType, IHitBoxRegion>(item.HitBoxType)
            };

            result.Add(item.Orientation, hitBoxRegion);
        }

        return result;

        RectangularHitBoxRegion CreateRectangularHitBoxRegion(
            GadgetData gadgetData,
            ReadOnlySpan<Point> hitBoxRegionData)
        {
            if (hitBoxRegionData.Length != 2)
                ThrowInvalidHitBoxRegionDataException();

            var transformationData = new DihedralTransformation.TransformationData(gadgetData.Orientation, gadgetData.FacingDirection, SpriteData.BaseSpriteSize);

            var p0 = transformationData.Transform(hitBoxRegionData[0]);
            var p1 = transformationData.Transform(hitBoxRegionData[1]);

            return new RectangularHitBoxRegion(p0, p1);
        }

        [SkipLocalsInit]
        PointSetHitBoxRegion CreatePointSetHitBoxRegion(
            GadgetData gadgetData,
            ReadOnlySpan<Point> triggerData)
        {
            var transformationData = new DihedralTransformation.TransformationData(gadgetData.Orientation, gadgetData.FacingDirection, SpriteData.BaseSpriteSize);

            Span<Point> adjustedPoints = triggerData.Length > 32
                ? new Point[triggerData.Length]
                : stackalloc Point[triggerData.Length];

            for (var i = 0; i < triggerData.Length; i++)
            {
                adjustedPoints[i] = transformationData.Transform(triggerData[i]);
            }

            return new PointSetHitBoxRegion(adjustedPoints);
        }

        ResizableRectangularHitBoxRegion CreateResizableRectangularHitBoxRegion(
            GadgetBounds hitBoxGadgetBounds,
            ReadOnlySpan<Point> hitBoxRegionData)
        {
            if (hitBoxRegionData.Length != 2)
                ThrowInvalidHitBoxRegionDataException();

            var transformationData = new DihedralTransformation.TransformationData(gadgetData.Orientation, gadgetData.FacingDirection, SpriteData.BaseSpriteSize);

            var p0 = transformationData.Transform(hitBoxRegionData[0]);
            var p1 = transformationData.Transform(hitBoxRegionData[1]);

            return new ResizableRectangularHitBoxRegion(hitBoxGadgetBounds, p0.X, p0.Y, p1.X, p1.Y);
        }
    }

    [DoesNotReturn]
    private static void ThrowInvalidHitBoxRegionDataException()
    {
        throw new InvalidOperationException("Expected exactly two points of data");
    }
}
