using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Rewind;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2;

public static class LemmingBehaviourBuilder
{
    public static LemmingBehaviour BuildLemmingBehaviour(
        DataHandle dataHandle,
        int newBehaviourId,
        in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var lemmingBehaviourType = (LemmingBehaviourType)gadgetBehaviourDatum.DataChunk.Data1.GetRawValue();

        return lemmingBehaviourType switch
        {
            LemmingBehaviourType.ChangeLemmingAbility => BuildChangeAbilityLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.ClearAllLemmingAbilities => BuildClearAllStatesLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.SetLemmingAction => BuildSetActionLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.SetLemmingTribe => BuildSetTribeLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.SkillCountChange => BuildLemmingSpecificSkillCountChangeBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.KillLemming => BuildKillLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.ForceLemmingFacingDirection => BuildForceFacingDirectionLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.NullifyLemmingFallDistance => BuildNullifyFallDistanceLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.MoveLemming => BuildMoveLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.SetLemmingPosition => BuildSetLemmingPositionBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.SetLemmingFastForward => BuildFastForwardLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),

            _ => Helpers.ThrowUnknownEnumValueException<LemmingBehaviourType, LemmingBehaviour>(lemmingBehaviourType),
        };
    }

    private static ChangeAbilityLemmingBehaviour BuildChangeAbilityLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        PackedBytes32 stateData = gadgetBehaviourDatum.DataChunk.Data2;

        int lemmingStateChangerId = stateData.GetUshort(0);
        var lemmingStateChanger = ILemmingAbilityChanger.AllItems[lemmingStateChangerId];

        uint rawSetStateTypeId = stateData.GetUshort(1);
        var setStateType = ChangeAbilityLemmingBehaviour.GetEnumValue(rawSetStateTypeId);

        return new ChangeAbilityLemmingBehaviour(lemmingStateChanger, setStateType)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }

    private static ClearAllSkillsLemmingBehaviour BuildClearAllStatesLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        return new ClearAllSkillsLemmingBehaviour()
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }

    private static SetActionLemmingBehaviour BuildSetActionLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        LemmingActionType lemmingAction = LemmingActionConstants.GetEnumValue(gadgetBehaviourDatum.DataChunk.Data2.GetRawValue());

        return new SetActionLemmingBehaviour(lemmingAction)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }

    private static SetTribeLemmingBehaviour BuildSetTribeLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        int lemmingTribeId = (int)gadgetBehaviourDatum.DataChunk.Data1.GetRawValue();

        return new SetTribeLemmingBehaviour(lemmingTribeId)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }

    private static LemmingSpecificSkillCountChangeBehaviour BuildLemmingSpecificSkillCountChangeBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        uint rawLemmingSkillType = gadgetBehaviourDatum.DataChunk.Data1.GetRawValue();
        LemmingSkillType lemmingSkill = LemmingSkillConstants.GetEnumValue(rawLemmingSkillType);
        int skillCountDelta = (int)gadgetBehaviourDatum.DataChunk.Data2.GetRawValue();

        return new LemmingSpecificSkillCountChangeBehaviour(lemmingSkill, skillCountDelta)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }

    private static KillLemmingBehaviour BuildKillLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        uint rawLemmingRemovalReasonId = gadgetBehaviourDatum.DataChunk.Data2.GetRawValue();
        LemmingRemovalReason lemmingRemovalReason = LemmingRemovalReasonHelpers.GetEnumValue(rawLemmingRemovalReasonId);

        return new KillLemmingBehaviour(lemmingRemovalReason)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }

    private static ForceFacingDirectionLemmingBehaviour BuildForceFacingDirectionLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        FacingDirection facingDirection = new(gadgetBehaviourDatum.DataChunk.Data2.GetByte(0));

        return new ForceFacingDirectionLemmingBehaviour(facingDirection)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }

    private static NullifyFallDistanceLemmingBehaviour BuildNullifyFallDistanceLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        return new NullifyFallDistanceLemmingBehaviour()
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }

    private static MoveLemmingBehaviour BuildMoveLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var delta = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.DataChunk.Data2);

        return new MoveLemmingBehaviour(delta)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }

    private static SetLemmingPositionBehaviour BuildSetLemmingPositionBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        Point position = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.DataChunk.Data2);

        PackedBytes32 miscData = gadgetBehaviourDatum.DataChunk.Data3;
        uint rawRelativePositioningType = miscData.GetByte(0);
        RelativePositioningType relativePositioningType = RelativePositioningTypeHelpers.GetEnumValue(rawRelativePositioningType);
        uint maxTriggerCountPerTick = miscData.GetByte(1);

        return new SetLemmingPositionBehaviour(position, relativePositioningType)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = maxTriggerCountPerTick,
        };
    }

    private static FastForwardLemmingBehaviour BuildFastForwardLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        uint fastForwardTime = gadgetBehaviourDatum.DataChunk.Data2.GetRawValue();

        return new FastForwardLemmingBehaviour(fastForwardTime)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }
}
