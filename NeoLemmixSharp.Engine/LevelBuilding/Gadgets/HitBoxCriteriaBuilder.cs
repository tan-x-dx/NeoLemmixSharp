using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering.Criteria;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class HitBoxCriteriaBuilder
{
    public static LemmingCriterion[] BuildLemmingCriteria(
        ReadOnlySpan<HitBoxCriteriaData> gadgetArchetypeDataHitBoxCriteriaData,
        ReadOnlySpan<HitBoxCriteriaData> gadgetInstanceDataHitBoxCriteriaData,
        BitArraySet<LemmingCriteriaHasher, BitBuffer32, LemmingCriteria> allowedCustomHitBoxCriteria,
        TribeManager tribeManager)
    {
        OrientationSet? orientationSet = null;
        int facingDirectionIds = 0;
        LemmingActionSet? lemmingActionSet = null;
        LemmingStateSet? lemmingStateSet = null;
        TribeSet? tribeSet = null;
        var numberOfCriteria = 0;

        foreach (var hitBoxCriteriaDatum in gadgetArchetypeDataHitBoxCriteriaData)
        {
            RegisterLemmingCriterion(hitBoxCriteriaDatum);
        }

        foreach (var hitBoxCriteriaDatum in gadgetInstanceDataHitBoxCriteriaData)
        {
            if (!allowedCustomHitBoxCriteria.Contains(hitBoxCriteriaDatum.LemmingCriteria))
                throw new InvalidOperationException("Not allowed to change this criterion!");

            RegisterLemmingCriterion(hitBoxCriteriaDatum);
        }

        CheckSetIsNotFull(ref orientationSet, ref numberOfCriteria);
        // Have to deal with facing directions differently since there's
        // only two possible values. It's not worth creating a set object
        // for just two values so we mess around with IDs directly.
        if (facingDirectionIds == 3)
        {
            facingDirectionIds = 0;
            numberOfCriteria--;
        }
        CheckSetIsNotFull(ref lemmingActionSet, ref numberOfCriteria);
        CheckSetIsNotFull(ref lemmingStateSet, ref numberOfCriteria);
        CheckSetIsNotFull(ref tribeSet, ref numberOfCriteria);

        return CreateLemmingCriteriaArray(orientationSet, facingDirectionIds, lemmingActionSet, lemmingStateSet, tribeSet, numberOfCriteria);

        void RegisterLemmingCriterion(HitBoxCriteriaData hitBoxCriteriaDatum)
        {
            switch (hitBoxCriteriaDatum.LemmingCriteria)
            {
                case LemmingCriteria.LemmingOrientation:
                    AddOrientationToCriteria(ref orientationSet, ref numberOfCriteria, hitBoxCriteriaDatum.ItemId);
                    break;

                case LemmingCriteria.LemmingFacingDirection:
                    AddFacingToCriteria(ref facingDirectionIds, ref numberOfCriteria, hitBoxCriteriaDatum.ItemId);
                    break;

                case LemmingCriteria.LemmingAction:
                    AddLemmingActionToCriteria(ref lemmingActionSet, ref numberOfCriteria, hitBoxCriteriaDatum.ItemId);
                    break;

                case LemmingCriteria.LemmingState:
                    AddLemmingStateToCriteria(ref lemmingStateSet, ref numberOfCriteria, hitBoxCriteriaDatum.ItemId);
                    break;

                case LemmingCriteria.LemmingTribe:
                    AddLemmingTribeToCriteria(ref tribeSet, ref numberOfCriteria, hitBoxCriteriaDatum.ItemId, tribeManager);
                    break;

                default:
                    Helpers.ThrowUnknownEnumValueException<LemmingCriteria, LemmingCriteria>(hitBoxCriteriaDatum.LemmingCriteria);
                    break;
            }
        }
    }

    private static LemmingCriterion[] CreateLemmingCriteriaArray(
        OrientationSet? orientationSet,
        int facingDirectionIds,
        LemmingActionSet? lemmingActionSet,
        LemmingStateSet? lemmingStateSet,
        TribeSet? tribeSet,
        int numberOfCriteria)
    {
        var result = Helpers.GetArrayForSize<LemmingCriterion>(numberOfCriteria);
        var i = 0;

        if (orientationSet is not null) result[i++] = new LemmingOrientationCriterion(orientationSet);
        if (facingDirectionIds != 0)
        {
            var id = facingDirectionIds == 1 ? EngineConstants.RightFacingDirectionId : EngineConstants.LeftFacingDirectionId;
            result[i++] = LemmingFacingDirectionCriterion.ForFacingDirection(id);
        }
        if (lemmingActionSet is not null) result[i++] = new LemmingActionCriterion(lemmingActionSet);
        if (lemmingStateSet is not null) result[i++] = new LemmingStateCriterion(lemmingStateSet);
        if (tribeSet is not null) result[i++] = new LemmingTribeCriterion(tribeSet);

        Debug.Assert(i == result.Length);

        return result;
    }

    private static void AddOrientationToCriteria(ref OrientationSet? orientationSet, ref int numberOfCriteria, int itemId)
    {
        if (orientationSet is null)
        {
            orientationSet = Orientation.CreateBitArraySet();
            numberOfCriteria++;
        }

        var orientation = new Orientation(itemId);
        orientationSet.Add(orientation);
    }

    private static void AddFacingToCriteria(ref int facingDirectionIds, ref int numberOfCriteria, int itemId)
    {
        var newFacingDirectionId = itemId & 1;
        if (facingDirectionIds == 0)
        {
            numberOfCriteria++;
            return;
        }

        facingDirectionIds |= 1 << newFacingDirectionId;
    }

    private static void AddLemmingActionToCriteria(ref LemmingActionSet? lemmingActionSet, ref int numberOfCriteria, int itemId)
    {
        if (lemmingActionSet is null)
        {
            lemmingActionSet = LemmingAction.CreateBitArraySet();
            numberOfCriteria++;
        }

        var lemmingAction = LemmingAction.AllItems[itemId];
        lemmingActionSet.Add(lemmingAction);
    }

    private static void AddLemmingStateToCriteria(ref LemmingStateSet? lemmingStateSet, ref int numberOfCriteria, int itemId)
    {
        if (lemmingStateSet is null)
        {
            lemmingStateSet = ILemmingState.CreateBitArraySet();
            numberOfCriteria++;
        }

        var lemmingState = ILemmingState.AllItems[itemId];
        lemmingStateSet.Add(lemmingState);
    }

    private static void AddLemmingTribeToCriteria(ref TribeSet? tribeSet, ref int numberOfCriteria, int itemId, TribeManager tribeManager)
    {
        if (tribeSet is null)
        {
            tribeSet = tribeManager.CreateBitArraySet();
            numberOfCriteria++;
        }

        var tribe = tribeManager.AllItems[itemId];
        tribeSet.Add(tribe);
    }

    // If the set contains all possible values, then it is functionally useless.
    // All queries will always return true, so we might as well not even bother
    // creating a criterion object.
    private static void CheckSetIsNotFull<TPerfectHasher, TBuffer, T>(ref BitArraySet<TPerfectHasher, TBuffer, T>? set, ref int numberOfCriteria)
        where TPerfectHasher : IBitBufferCreator<TBuffer, T>
        where TBuffer : struct, IBitBuffer
        where T : notnull
    {
        if (set is not null && set.IsFull)
        {
            set = null;
            numberOfCriteria--;
        }
    }
}
