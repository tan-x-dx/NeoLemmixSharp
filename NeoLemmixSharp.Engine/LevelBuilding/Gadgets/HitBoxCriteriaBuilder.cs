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
        HitBoxData hitBoxData,
        TribeManager tribeManager)
    {
        var result = CreateArrayForSize(hitBoxData);

        var i = 0;

        if (hitBoxData.AllowedLemmingActionIds.Length > 0)
            result[i++] = BuildLemmingActionCriterion(hitBoxData.AllowedLemmingActionIds);

        if (hitBoxData.AllowedLemmingStateIds.Length > 0)
            result[i++] = BuildLemmingStateCriterion(hitBoxData.AllowedLemmingStateIds);

        if (hitBoxData.AllowedLemmingTribeIds.HasValue)
            result[i++] = BuildLemmingTribeCriterion(hitBoxData.AllowedLemmingTribeIds.Value, tribeManager);

        if (hitBoxData.AllowedLemmingOrientationIds.HasValue)
            result[i++] = BuildLemmingOrientationCriterion(hitBoxData.AllowedLemmingOrientationIds.Value);

        if (hitBoxData.AllowedFacingDirectionId.HasValue)
            result[i++] = BuildLemmingFacingDirectionCriterion(hitBoxData.AllowedFacingDirectionId.Value);

        Debug.Assert(i == result.Length);

        return result;
    }

    private static LemmingCriterion[] CreateArrayForSize(HitBoxData hitBoxData)
    {
        var arraySize = (hitBoxData.AllowedLemmingActionIds.Length > 0 ? 1 : 0) +
                        (hitBoxData.AllowedLemmingStateIds.Length > 0 ? 1 : 0) +
                        (hitBoxData.AllowedLemmingTribeIds.HasValue ? 1 : 0) +
                        (hitBoxData.AllowedLemmingOrientationIds.HasValue ? 1 : 0) +
                        (hitBoxData.AllowedFacingDirectionId.HasValue ? 1 : 0);

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
        var stateChangerSet = ILemmingStateChanger.CreateBitArraySet();
        stateChangerSet.ReadFrom(allowedLemmingStateIds);

        return new LemmingStateCriterion(stateChangerSet);
    }

    private static LemmingTribeCriterion BuildLemmingTribeCriterion(byte value, TribeManager tribeManager)
    {
        var tribe = tribeManager.AllItems[value];

        return new LemmingTribeCriterion(tribe);
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
