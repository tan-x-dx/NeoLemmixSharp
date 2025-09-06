﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2;

public static class LemmingBehaviourBuilder
{
    public static LemmingBehaviour BuildLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var lemmingBehaviourType = (LemmingBehaviourType)gadgetBehaviourDatum.Data1;

        return lemmingBehaviourType switch
        {
            LemmingBehaviourType.None => throw new InvalidOperationException("Invalid Behaviour Type!"),

            LemmingBehaviourType.SetLemmingState => BuildSetStateLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.ClearLemmingStates => BuildClearAllStatesLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.SetLemmingAction => BuildSetActionLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.KillLemming => BuildKillLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.ForceLemmingFacingDirection => BuildForceFacingDirectionLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.NullifyLemmingFallDistance => BuildNullifyFallDistanceLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.LemmingMover => BuildMoveLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.SetLemmingFastForward => BuildFastForwardLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),

            _ => Helpers.ThrowUnknownEnumValueException<LemmingBehaviourType, LemmingBehaviour>(lemmingBehaviourType),
        };
    }

    private static SetStateLemmingBehaviour BuildSetStateLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var lemmingStateChangerId = gadgetBehaviourDatum.Data2 & 0xffff;
        var lemmingStateChanger = ILemmingState.AllItems[lemmingStateChangerId];

        var rawSetStateTypeId = (uint)(gadgetBehaviourDatum.Data2 >>> 16);
        var setStateType = SetStateLemmingBehaviour.GetEnumValue(rawSetStateTypeId);

        return new SetStateLemmingBehaviour(lemmingStateChanger, setStateType)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static ClearAllSkillsLemmingBehaviour BuildClearAllStatesLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        return new ClearAllSkillsLemmingBehaviour()
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static SetActionLemmingBehaviour BuildSetActionLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var lemmingAction = LemmingAction.AllItems[gadgetBehaviourDatum.Data2];

        return new SetActionLemmingBehaviour(lemmingAction)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static KillLemmingBehaviour BuildKillLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var rawLemmingRemovalReasonId = (uint)gadgetBehaviourDatum.Data2;
        var lemmingRemovalReason = LemmingRemovalReasonHelpers.GetEnumValue(rawLemmingRemovalReasonId);

        return new KillLemmingBehaviour(lemmingRemovalReason)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static ForceFacingDirectionLemmingBehaviour BuildForceFacingDirectionLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var facingDirection = new FacingDirection(gadgetBehaviourDatum.Data2);

        return new ForceFacingDirectionLemmingBehaviour(facingDirection)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static NullifyFallDistanceLemmingBehaviour BuildNullifyFallDistanceLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        return new NullifyFallDistanceLemmingBehaviour()
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static MoveLemmingBehaviour BuildMoveLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var delta = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.Data2);

        return new MoveLemmingBehaviour(delta)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static FastForwardLemmingBehaviour BuildFastForwardLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var fastForwardTime = gadgetBehaviourDatum.Data2;

        return new FastForwardLemmingBehaviour(fastForwardTime)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }
}
