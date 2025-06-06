using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class HitBoxCriteriaBuilder
{
    public static LemmingCriterion[] BuildLemmingCriteria(
        HitBoxCriteriaData gadgetArchetypeDataHitBoxCriteriaData,
        HitBoxCriteriaData? gadgetDataOverrideHitBoxCriteriaData,
        TribeManager tribeManager)
    {
        var lemmingCriteriaData = gadgetDataOverrideHitBoxCriteriaData ?? gadgetArchetypeDataHitBoxCriteriaData;

        var result = CreateArrayForSize(lemmingCriteriaData);

        var i = 0;

        if (lemmingCriteriaData.AllowedLemmingActionIds.Length > 0)
            result[i++] = BuildLemmingActionCriterion(lemmingCriteriaData.AllowedLemmingActionIds);

        if (lemmingCriteriaData.AllowedLemmingStateIds.Length > 0)
            result[i++] = BuildLemmingStateCriterion(lemmingCriteriaData.AllowedLemmingStateIds);

        if (lemmingCriteriaData.AllowedLemmingTribeIds != 0)
            result[i++] = BuildLemmingTribeCriterion(lemmingCriteriaData.AllowedLemmingTribeIds, tribeManager);

        if (lemmingCriteriaData.AllowedLemmingOrientationIds != 0)
            result[i++] = BuildLemmingOrientationCriterion(lemmingCriteriaData.AllowedLemmingOrientationIds);

        if (lemmingCriteriaData.AllowedFacingDirectionId != 0)
            result[i++] = BuildLemmingFacingDirectionCriterion(lemmingCriteriaData.AllowedFacingDirectionId);

        Debug.Assert(i == result.Length);

        return result;
    }

    private static LemmingCriterion[] CreateArrayForSize(HitBoxCriteriaData lemmingCriteriaData)
    {
        var arraySize = (lemmingCriteriaData.AllowedLemmingActionIds.Length > 0 ? 1 : 0) +
                        (lemmingCriteriaData.AllowedLemmingStateIds.Length > 0 ? 1 : 0) +
                        (lemmingCriteriaData.AllowedLemmingTribeIds != 0 ? 1 : 0) +
                        (lemmingCriteriaData.AllowedLemmingOrientationIds != 0 ? 1 : 0) +
                        (lemmingCriteriaData.AllowedFacingDirectionId != 0 ? 1 : 0);

        return CollectionsHelper.GetArrayForSize<LemmingCriterion>(arraySize);
    }

    private static LemmingActionCriterion BuildLemmingActionCriterion(uint[] allowedLemmingActionIds)
    {
        var lemmingActionSet = LemmingAction.CreateBitArraySet();
        lemmingActionSet.ReadFrom(allowedLemmingActionIds);

        return new LemmingActionCriterion(lemmingActionSet);
    }

    private static LemmingStateCriterion BuildLemmingStateCriterion(uint[] allowedLemmingStateIds)
    {
        var lemmingStateSet = ILemmingState.CreateBitArraySet();
        lemmingStateSet.ReadFrom(allowedLemmingStateIds);

        return new LemmingStateCriterion(lemmingStateSet);
    }

    private static LemmingTribeCriterion BuildLemmingTribeCriterion(byte value, TribeManager tribeManager)
    {
        var tribeSet = tribeManager.CreateBitArraySet();
        uint intValue = value;
        var sourceSpan = new ReadOnlySpan<uint>(ref intValue);
        tribeSet.ReadFrom(sourceSpan);

        return new LemmingTribeCriterion(tribeSet);
    }

    private static LemmingOrientationCriterion BuildLemmingOrientationCriterion(byte value)
    {
        var orientationSet = Orientation.CreateBitArraySet();
        uint intValue = value;
        var sourceSpan = new ReadOnlySpan<uint>(ref intValue);
        orientationSet.ReadFrom(sourceSpan);

        return new LemmingOrientationCriterion(orientationSet);
    }

    private static LemmingFacingDirectionCriterion BuildLemmingFacingDirectionCriterion(byte value)
    {
        return LemmingFacingDirectionCriterion.ForFacingDirection(value);
    }
}
