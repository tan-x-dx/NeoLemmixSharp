using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.GadgetAnimation;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Global;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Movement;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.GadgetBehaviours;
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

    public Dictionary<GadgetBehaviourName, GadgetBehaviour> BuildBehaviourLookup(
        DataHandle dataHandle,
        ReadOnlySpan<GadgetBehaviourData> innateBehaviours,
        ReadOnlySpan<GadgetBehaviourData> customBehaviours)
    {
        var result = new Dictionary<GadgetBehaviourName, GadgetBehaviour>(innateBehaviours.Length + customBehaviours.Length);

        for (var a = 0; a < innateBehaviours.Length; a++)
        {
            ref readonly var gadgetBehaviourDatum = ref innateBehaviours[a];
            var newGadgetBehaviour = BuildBehaviour(dataHandle, in gadgetBehaviourDatum);
            result.Add(newGadgetBehaviour.GadgetBehaviourName, newGadgetBehaviour);
        }

        for (var a = 0; a < customBehaviours.Length; a++)
        {
            ref readonly var gadgetBehaviourDatum = ref customBehaviours[a];
            var newGadgetBehaviour = BuildBehaviour(dataHandle, in gadgetBehaviourDatum);
            result.Add(newGadgetBehaviour.GadgetBehaviourName, newGadgetBehaviour);
        }

        return result;
    }

    public GadgetBehaviour[] BuildBehaviours(
        DataHandle dataHandle,
        ReadOnlySpan<GadgetBehaviourData> innateBehaviours,
        ReadOnlySpan<GadgetBehaviourData> customBehaviours)
    {
        var result = Helpers.GetArrayForSize<GadgetBehaviour>(innateBehaviours.Length + customBehaviours.Length);
        var i = 0;

        for (var a = 0; a < innateBehaviours.Length; a++)
        {
            ref readonly var gadgetBehaviourDatum = ref innateBehaviours[a];
            var newGadgetBehaviour = BuildBehaviour(dataHandle, in gadgetBehaviourDatum);
            result[i++] = newGadgetBehaviour;
        }

        for (var a = 0; a < customBehaviours.Length; a++)
        {
            ref readonly var gadgetBehaviourDatum = ref customBehaviours[a];
            var newGadgetBehaviour = BuildBehaviour(dataHandle, in gadgetBehaviourDatum);
            result[i++] = newGadgetBehaviour;
        }

        return result;
    }

    private GadgetBehaviour BuildBehaviour(
        DataHandle dataHandle,
        in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var newBehaviourId = _gadgetBehaviours.Count;

        GadgetBehaviour newGadgetBehaviour = gadgetBehaviourDatum.GadgetBehaviourType switch
        {
            GadgetBehaviourType.None => throw new InvalidOperationException("Invalid Behaviour Type!"),

            GadgetBehaviourType.GadgetOutputSignal => BuildGadgetOutputSignalBehaviour(dataHandle, newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetChangeInternalState => BuildGadgetChangeInternalStateBehaviour(dataHandle, newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetMove => BuildMoveGadgetBehaviour(dataHandle, newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetFreeResize => BuildFreeResizeGadgetBehaviour(dataHandle, newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetConstrainedResize => BuildConstrainedResizeGadgetBehaviour(dataHandle, newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GadgetAnimationRenderLayer => BuildGadgetAnimationRenderLayerBehaviour(dataHandle, newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.LemmingBehaviour => LemmingBehaviourBuilder.BuildLemmingBehaviour(dataHandle, newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GlobalAdditionalTime => BuildGlobalAdditionalTimeBehaviour(dataHandle, newBehaviourId, in gadgetBehaviourDatum),
            GadgetBehaviourType.GlobalSkillCountChange => BuildGlobalSkillCountChangeBehaviour(dataHandle, newBehaviourId, in gadgetBehaviourDatum),

            _ => Helpers.ThrowUnknownEnumValueException<GadgetBehaviourType, GadgetBehaviour>(gadgetBehaviourDatum.GadgetBehaviourType),
        };

        _gadgetBehaviours.Add(newGadgetBehaviour);

        return newGadgetBehaviour;
    }

    private static OutputSignalBehaviour BuildGadgetOutputSignalBehaviour(
        DataHandle dataHandle,
        int newBehaviourId,
        in GadgetBehaviourData gadgetBehaviourDatum)
    {
        return new OutputSignalBehaviour()
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = 1
        };
    }

    private static StateChangeBehaviour BuildGadgetChangeInternalStateBehaviour(
        DataHandle dataHandle,
        int newBehaviourId,
        in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var intendedStateIndex = gadgetBehaviourDatum.DataChunk.Data2;

        return new StateChangeBehaviour(intendedStateIndex)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = 1
        };
    }

    private MoveGadgetBehaviour BuildMoveGadgetBehaviour(
        DataHandle dataHandle,
        int newBehaviourId,
        in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var maxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data1 & 0xffff;
        var tickDelay = gadgetBehaviourDatum.DataChunk.Data1 >>> 16;
        var delta = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.DataChunk.Data1);
        var limit = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.DataChunk.Data2);

        return new MoveGadgetBehaviour(dataHandle, _gadgetIdentifier, tickDelay, delta, limit)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = maxTriggerCountPerTick
        };
    }

    private FreeResizeHitBoxGadgetBehaviour BuildFreeResizeGadgetBehaviour(
        DataHandle dataHandle,
        int newBehaviourId,
        in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var tickDelay = gadgetBehaviourDatum.DataChunk.Data1;
        var delta = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.DataChunk.Data2);

        return new FreeResizeHitBoxGadgetBehaviour(dataHandle, _gadgetIdentifier, tickDelay, delta.X, delta.Y)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3
        };
    }

    private ConstrainedResizeHitBoxGadgetBehaviour BuildConstrainedResizeGadgetBehaviour(
        DataHandle dataHandle,
        int newBehaviourId,
        in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var maxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data1 & 0xffff;
        var tickDelay = gadgetBehaviourDatum.DataChunk.Data1 >>> 16;
        var delta = gadgetBehaviourDatum.DataChunk.Data1;
        var max = gadgetBehaviourDatum.DataChunk.Data2;

        return new ConstrainedResizeHitBoxGadgetBehaviour(dataHandle, _gadgetIdentifier, tickDelay, delta, max)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = maxTriggerCountPerTick
        };
    }

    private static AnimationLayerBehaviour BuildGadgetAnimationRenderLayerBehaviour(
        DataHandle dataHandle,
        int newBehaviourId,
        in GadgetBehaviourData gadgetBehaviourDatum)
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
                dataHandle,
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
                dataHandle,
                newBehaviourId,
                gadgetBehaviourDatum.GadgetBehaviourName,
                layer,
                minFrame,
                maxFrame,
                initialFrame,
                gadgetLayerColorData);
        }
    }

    private static AdditionalTimeBehaviour BuildGlobalAdditionalTimeBehaviour(
        DataHandle dataHandle,
        int newBehaviourId,
        in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var additionalTimeInSecods = gadgetBehaviourDatum.DataChunk.Data2;

        return new AdditionalTimeBehaviour(additionalTimeInSecods)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3
        };
    }

    private static SkillCountChangeBehaviour BuildGlobalSkillCountChangeBehaviour(
        DataHandle dataHandle,
        int newBehaviourId,
        in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var skillCountDelta = gadgetBehaviourDatum.DataChunk.Data2 & 0xffff;
        var tribeId = gadgetBehaviourDatum.DataChunk.Data2 >>> 16;
        var lemmingSkillType = (uint)gadgetBehaviourDatum.DataChunk.Data1;
        var lemmingSkill = LemmingSkillConstants.GetEnumValue(lemmingSkillType);

        return new SkillCountChangeBehaviour(lemmingSkill, tribeId, skillCountDelta)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3
        };
    }
}
