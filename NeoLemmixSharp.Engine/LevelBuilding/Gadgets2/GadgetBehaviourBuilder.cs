using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.GadgetAnimation;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Global;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Movement;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.GadgetBehaviours;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Util;
using System.Runtime.CompilerServices;

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
            GadgetBehaviourType.GadgetMove => BuildMoveGadgetBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetFreeResize => BuildFreeResizeGadgetBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetConstrainedResize => BuildConstrainedResizeGadgetBehaviour(newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetAnimationRenderLayer => BuildGadgetAnimationRenderLayerBehaviour(newBehaviourId, in gadgetBehaviourDatum),
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
        return new OutputSignalBehaviour()
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = 1
        };
    }

    private static StateChangeBehaviour BuildGadgetChangeInternalStateBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var intendedStateIndex = gadgetBehaviourDatum.DataChunk.Data2;

        return new StateChangeBehaviour(intendedStateIndex)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = 1
        };
    }

    private MoveGadgetBehaviour BuildMoveGadgetBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var maxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data1 & 0xffff;
        var tickDelay = gadgetBehaviourDatum.DataChunk.Data1 >>> 16;
        var delta = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.DataChunk.Data1);
        var limit = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.DataChunk.Data2);

        return new MoveGadgetBehaviour(_gadgetIdentifier, tickDelay, delta, limit)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = maxTriggerCountPerTick
        };
    }

    private FreeResizeHitBoxGadgetBehaviour BuildFreeResizeGadgetBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var tickDelay = gadgetBehaviourDatum.DataChunk.Data1;
        var delta = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.DataChunk.Data2);

        return new FreeResizeHitBoxGadgetBehaviour(_gadgetIdentifier, tickDelay, delta.X, delta.Y)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3
        };
    }

    private ConstrainedResizeHitBoxGadgetBehaviour BuildConstrainedResizeGadgetBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var maxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data1 & 0xffff;
        var tickDelay = gadgetBehaviourDatum.DataChunk.Data1 >>> 16;
        var delta = gadgetBehaviourDatum.DataChunk.Data1;
        var max = gadgetBehaviourDatum.DataChunk.Data2;

        return new ConstrainedResizeHitBoxGadgetBehaviour(_gadgetIdentifier, tickDelay, delta, max)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = maxTriggerCountPerTick
        };
    }

    private static AnimationLayerBehaviour BuildGadgetAnimationRenderLayerBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var frameData = gadgetBehaviourDatum.DataChunk.Data1;

        var minFrame = frameData & 0xff;
        frameData >>>= 8;
        var maxFrame = frameData & 0xff;
        frameData >>>= 8;
        var initialFrame = frameData & 0xff;
        frameData >>>= 8;
        var layer = frameData & 0xff;

        var layerColorData = gadgetBehaviourDatum.DataChunk.Data2;
        var isIncrement = (layerColorData & 1) == 0;
        layerColorData >>>= 1;
        var isTribeColorLayer = (layerColorData & 1) != 0;

        GadgetLayerColorData gadgetLayerColorData;
        if (isTribeColorLayer)
        {
            var rawTribeData = gadgetBehaviourDatum.DataChunk.Data3;
            var tribeId = rawTribeData & 0xff;
            rawTribeData >>>= 8;
            var tribeColorLayerType = TribeSpriteLayerColorTypeHelpers.GetEnumValue((uint)(rawTribeData & 0xff));
            gadgetLayerColorData = new GadgetLayerColorData(tribeId, tribeColorLayerType);
        }
        else
        {
            var rawColor = gadgetBehaviourDatum.DataChunk.Data3;
            var color = Unsafe.As<int, Color>(ref rawColor);
            gadgetLayerColorData = new GadgetLayerColorData(color);
        }

        if (isIncrement)
        {
            return AnimationLayerBehaviour.CreateIncrementAnimationLayerBehaviour(
                newBehaviourId,
                gadgetBehaviourDatum.GadgetBehaviourName,
                layer,
                minFrame,
                maxFrame,
                initialFrame,
                gadgetLayerColorData);
        }
        else
        {
            return AnimationLayerBehaviour.CreateDecrementAnimationLayerBehaviour(
                newBehaviourId,
                gadgetBehaviourDatum.GadgetBehaviourName,
                layer,
                minFrame,
                maxFrame,
                initialFrame,
                gadgetLayerColorData);
        }
    }

    private static AdditionalTimeBehaviour BuildGlobalAdditionalTimeBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var additionalTimeInSecods = gadgetBehaviourDatum.DataChunk.Data2;

        return new AdditionalTimeBehaviour(additionalTimeInSecods)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3
        };
    }

    private static SkillCountChangeBehaviour BuildGlobalSkillCountChangeBehaviour(int newBehaviourId, in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var lemmingSkillId = gadgetBehaviourDatum.DataChunk.Data1;
        var skillCountDelta = gadgetBehaviourDatum.DataChunk.Data2 & 0xffff;
        var overrideTribeId = gadgetBehaviourDatum.DataChunk.Data2 >>> 16;
        var lemmingSkill = LemmingSkill.GetSkillOrDefault(lemmingSkillId);

        return new SkillCountChangeBehaviour(lemmingSkill, overrideTribeId, skillCountDelta)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3
        };
    }
}
