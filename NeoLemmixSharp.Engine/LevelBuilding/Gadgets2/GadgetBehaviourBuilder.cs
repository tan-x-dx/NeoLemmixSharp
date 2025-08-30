using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2;

public readonly struct GadgetBehaviourBuilder
{
    private readonly GadgetIdentifier _gadgetIdentifier;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public GadgetBehaviourBuilder(
        GadgetIdentifier gadgetIdentifier,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _gadgetIdentifier = gadgetIdentifier;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public Dictionary<GadgetBehaviourName, GadgetBehaviour> BuildBehaviourLookup(ReadOnlySpan<GadgetBehaviourData> innateBehaviours, ReadOnlySpan<GadgetBehaviourData> customBehaviours)
    {
        var result = new Dictionary<GadgetBehaviourName, GadgetBehaviour>(innateBehaviours.Length + customBehaviours.Length);

        for (var a = 0; a < innateBehaviours.Length; a++)
        {
            ref readonly var gadgetBehaviourDatum = ref innateBehaviours[a];
            var newGadgetBehaviour = BuildBehaviour(in gadgetBehaviourDatum);
            result.Add(newGadgetBehaviour.GadgetBehaviourName, newGadgetBehaviour);
        }

        for (var a = 0; a < customBehaviours.Length; a++)
        {
            ref readonly var gadgetBehaviourDatum = ref customBehaviours[a];
            var newGadgetBehaviour = BuildBehaviour(in gadgetBehaviourDatum);
            result.Add(newGadgetBehaviour.GadgetBehaviourName, newGadgetBehaviour);
        }

        return result;
    }

    public GadgetBehaviour[] BuildBehaviours(ReadOnlySpan<GadgetBehaviourData> innateBehaviours, ReadOnlySpan<GadgetBehaviourData> customBehaviours)
    {
        var result = Helpers.GetArrayForSize<GadgetBehaviour>(innateBehaviours.Length + customBehaviours.Length);
        var i = 0;

        for (var a = 0; a < innateBehaviours.Length; a++)
        {
            ref readonly var gadgetBehaviourDatum = ref innateBehaviours[a];
            var newGadgetBehaviour = BuildBehaviour(in gadgetBehaviourDatum);
            result[i++] = newGadgetBehaviour;
        }

        for (var a = 0; a < customBehaviours.Length; a++)
        {
            ref readonly var gadgetBehaviourDatum = ref customBehaviours[a];
            var newGadgetBehaviour = BuildBehaviour(in gadgetBehaviourDatum);
            result[i++] = newGadgetBehaviour;
        }

        return result;
    }

    private GadgetBehaviour BuildBehaviour(in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var newBehaviourId = _gadgetBehaviours.Count;

        GadgetBehaviour newGadgetBehaviour = gadgetBehaviourDatum.GadgetBehaviourType switch
        {
            GadgetBehaviourType.None => throw new InvalidOperationException("Invalid Behaviour Type!"),

            GadgetBehaviourType.GadgetOutputSignal => CreateGadgetOutputSignalBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetChangeInternalState => CreateGadgetChangeInternalStateBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetMoveFree => CreateGadgetMoveFreeBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetMoveUntilPosition => CreateGadgetMoveUntilPositionBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetResizeFree => CreateGadgetResizeFreeBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetConstrainedResize => CreateGadgetConstrainedResizeBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetAnimationRenderLayer => CreateGadgetAnimationRenderLayerBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetAnimationSetFrame => CreateGadgetAnimationSetFrameBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetAnimationIncrementFrame => CreateGadgetAnimationIncrementFrameBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetAnimationDecrementFrame => CreateGadgetAnimationDecrementFrameBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.LemmingBehaviour => CreateLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GlobalAdditionalTime => CreateGlobalAdditionalTimeBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GlobalSkillCountChange => CreateGlobalSkillCountChangeBehaviour(newBehaviourId, in gadgetBehaviourDatum),

            _ => Helpers.ThrowUnknownEnumValueException<GadgetBehaviourType, GadgetBehaviour>(gadgetBehaviourDatum.GadgetBehaviourType),
        };

        _gadgetBehaviours.Add(newGadgetBehaviour);

        return newGadgetBehaviour;
    }

    private static GadgetBehaviour CreateGadgetOutputSignalBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour CreateGadgetChangeInternalStateBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private MoveFreeGadgetBehaviour CreateGadgetMoveFreeBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var tickDelay = gadgetBehaviourDatum.Data1;
        var dx = gadgetBehaviourDatum.Data2;
        var dy = gadgetBehaviourDatum.Data3;

        return new MoveFreeGadgetBehaviour(_gadgetIdentifier, tickDelay, dx, dy)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static GadgetBehaviour CreateGadgetMoveUntilPositionBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour CreateGadgetResizeFreeBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour CreateGadgetConstrainedResizeBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour CreateGadgetAnimationRenderLayerBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour CreateGadgetAnimationSetFrameBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour CreateGadgetAnimationIncrementFrameBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour CreateGadgetAnimationDecrementFrameBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static LemmingBehaviour CreateLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var lemmingBehaviourType = (LemmingBehaviourType)gadgetBehaviourDatum.Data1;

        return lemmingBehaviourType switch
        {
            LemmingBehaviourType.None => throw new InvalidOperationException("Invalid Behaviour Type!"),

            LemmingBehaviourType.SetLemmingState => CreateSetStateLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.ClearLemmingStates => CreateClearAllStatesLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.SetLemmingAction => CreateSetActionLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.KillLemming => CreateKillLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.ForceLemmingFacingDirection => CreateForceFacingDirectionLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.NullifyLemmingFallDistance => CreateNullifyFallDistanceLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            LemmingBehaviourType.LemmingMover => CreateMoveLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),

            _ => Helpers.ThrowUnknownEnumValueException<LemmingBehaviourType, LemmingBehaviour>(lemmingBehaviourType),
        };
    }

    private static SetStateLemmingBehaviour CreateSetStateLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
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

    private static ClearAllSkillsLemmingBehaviour CreateClearAllStatesLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        return new ClearAllSkillsLemmingBehaviour()
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static SetActionLemmingBehaviour CreateSetActionLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var lemmingAction = LemmingAction.AllItems[gadgetBehaviourDatum.Data2];

        return new SetActionLemmingBehaviour(lemmingAction)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static KillLemmingBehaviour CreateKillLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
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

    private static ForceFacingDirectionLemmingBehaviour CreateForceFacingDirectionLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var facingDirection = new FacingDirection(gadgetBehaviourDatum.Data2);

        return new ForceFacingDirectionLemmingBehaviour(facingDirection)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static NullifyFallDistanceLemmingBehaviour CreateNullifyFallDistanceLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        return new NullifyFallDistanceLemmingBehaviour()
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static MoveLemmingBehaviour CreateMoveLemmingBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var delta = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.Data2);

        return new MoveLemmingBehaviour(delta)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static GadgetBehaviour CreateGlobalAdditionalTimeBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour CreateGlobalSkillCountChangeBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }
}
