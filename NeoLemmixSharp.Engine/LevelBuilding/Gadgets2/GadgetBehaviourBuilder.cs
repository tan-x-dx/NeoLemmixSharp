using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Global;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Movement;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.GadgetBehaviours;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2;

public readonly ref struct GadgetBehaviourBuilder
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

            GadgetBehaviourType.GadgetOutputSignal => BuildGadgetOutputSignalBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetChangeInternalState => BuildGadgetChangeInternalStateBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetFreeMove => BuildGadgetMoveFreeBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetConstrainedMove => BuildConstrainedMoveGadgetBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetFreeResize => BuildFreeResizeGadgetBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetConstrainedResize => BuildConstrainedResizeGadgetBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetAnimationRenderLayer => BuildGadgetAnimationRenderLayerBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetAnimationSetFrame => BuildGadgetAnimationSetFrameBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetAnimationIncrementFrame => BuildGadgetAnimationIncrementFrameBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetAnimationDecrementFrame => BuildGadgetAnimationDecrementFrameBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.LemmingBehaviour => LemmingBehaviourBuilder.BuildLemmingBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GlobalAdditionalTime => BuildGlobalAdditionalTimeBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GlobalSkillCountChange => BuildGlobalSkillCountChangeBehaviour(newBehaviourId, in gadgetBehaviourDatum),

            _ => Helpers.ThrowUnknownEnumValueException<GadgetBehaviourType, GadgetBehaviour>(gadgetBehaviourDatum.GadgetBehaviourType),
        };

        _gadgetBehaviours.Add(newGadgetBehaviour);

        return newGadgetBehaviour;
    }

    private static OutputSignalBehaviour BuildGadgetOutputSignalBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour BuildGadgetChangeInternalStateBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private FreeMoveGadgetBehaviour BuildGadgetMoveFreeBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var tickDelay = gadgetBehaviourDatum.Data1;
        var delta = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.Data2);

        return new FreeMoveGadgetBehaviour(_gadgetIdentifier, tickDelay, delta)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private ConstrainedMoveGadgetBehaviour BuildConstrainedMoveGadgetBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var maxTriggerCountPerTick = gadgetBehaviourDatum.Data1 & 0xffff;
        var tickDelay = gadgetBehaviourDatum.Data1 >>> 16;
        var delta = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.Data1);
        var limit = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.Data2);

        return new ConstrainedMoveGadgetBehaviour(_gadgetIdentifier, tickDelay, delta, limit)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = maxTriggerCountPerTick
        };
    }

    private FreeResizeHitBoxGadgetBehaviour BuildFreeResizeGadgetBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var tickDelay = gadgetBehaviourDatum.Data1;
        var delta = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.Data2);

        return new FreeResizeHitBoxGadgetBehaviour(_gadgetIdentifier, tickDelay, delta.X, delta.Y)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private ConstrainedResizeHitBoxGadgetBehaviour BuildConstrainedResizeGadgetBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var maxTriggerCountPerTick = gadgetBehaviourDatum.Data1 & 0xffff;
        var tickDelay = gadgetBehaviourDatum.Data1 >>> 16;
        var delta = gadgetBehaviourDatum.Data1;
        var max = gadgetBehaviourDatum.Data2;

        return new ConstrainedResizeHitBoxGadgetBehaviour(_gadgetIdentifier, tickDelay, delta, max)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = maxTriggerCountPerTick
        };
    }

    private static GadgetBehaviour BuildGadgetAnimationRenderLayerBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour BuildGadgetAnimationSetFrameBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour BuildGadgetAnimationIncrementFrameBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour BuildGadgetAnimationDecrementFrameBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        throw new NotImplementedException();
    }

    private static AdditionalTimeBehaviour BuildGlobalAdditionalTimeBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var additionalTimeInSecods = gadgetBehaviourDatum.Data2;

        return new AdditionalTimeBehaviour(additionalTimeInSecods)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }

    private static SkillCountChangeBehaviour BuildGlobalSkillCountChangeBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var lemmingSkillId = gadgetBehaviourDatum.Data1;
        var skillCountDelta = gadgetBehaviourDatum.Data2 & 0xffff;
        var overrideTribeId = gadgetBehaviourDatum.Data2 >>> 16;
        var lemmingSkill = LemmingSkill.AllItems[lemmingSkillId];

        return new SkillCountChangeBehaviour(lemmingSkill, overrideTribeId, skillCountDelta)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.Data3
        };
    }
}
