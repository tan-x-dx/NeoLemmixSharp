using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Rewind;
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
        var intendedStateIndex = gadgetBehaviourDatum.DataChunk.Data2.Byte0;

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
        PackedBytes32 data1 = gadgetBehaviourDatum.DataChunk.Data1;
        uint maxTriggerCountPerTick = data1.Ushort0;
        int tickDelay = data1.Ushort1;
        Common.Point delta = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.DataChunk.Data2);
        Common.Point limit = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.DataChunk.Data3);

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
        int tickDelay = gadgetBehaviourDatum.DataChunk.Data1.Ushort0;
        Common.Point delta = ReadWriteHelpers.DecodePoint(gadgetBehaviourDatum.DataChunk.Data2);

        return new FreeResizeHitBoxGadgetBehaviour(dataHandle, _gadgetIdentifier, tickDelay, delta.X, delta.Y)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }

    private ConstrainedResizeHitBoxGadgetBehaviour BuildConstrainedResizeGadgetBehaviour(
        DataHandle dataHandle,
        int newBehaviourId,
        in GadgetBehaviourData gadgetBehaviourDatum)
    {
        PackedBytes32 data1 = gadgetBehaviourDatum.DataChunk.Data1;
        uint maxTriggerCountPerTick = data1.Ushort0;
        uint tickDelay = data1.Ushort1;
        int delta = (int)gadgetBehaviourDatum.DataChunk.Data2.GetRawValue();
        int max = (int)gadgetBehaviourDatum.DataChunk.Data3.GetRawValue();

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
        PackedBytes32 frameData = gadgetBehaviourDatum.DataChunk.Data1;

        int minFrame = frameData.Byte0;
        int maxFrame = frameData.Byte1;
        int initialFrame = frameData.Byte2;
        int layer = frameData.Byte3;

        PackedBytes32 layerColorData = gadgetBehaviourDatum.DataChunk.Data2;
        bool isIncrement = layerColorData.GetBit(0);
        bool isTribeColorLayer = layerColorData.GetBit(1);

        GadgetLayerColorData gadgetLayerColorData;

        if (isTribeColorLayer)
        {
            PackedBytes32 rawTribeData = gadgetBehaviourDatum.DataChunk.Data3;
            int tribeId = rawTribeData.Byte0;
            var tribeColorLayerType = TribeSpriteLayerColorTypeHelpers.GetEnumValue(rawTribeData.Byte1);
            gadgetLayerColorData = new GadgetLayerColorData(tribeId, tribeColorLayerType);
        }
        else
        {
            var rawColor = gadgetBehaviourDatum.DataChunk.Data3;
            var color = Unsafe.As<PackedBytes32, Color>(ref rawColor);
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

        return new AdditionalTimeBehaviour((int)additionalTimeInSecods.GetRawValue())
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }

    private static SkillCountChangeBehaviour BuildGlobalSkillCountChangeBehaviour(
        DataHandle dataHandle,
        int newBehaviourId,
        in GadgetBehaviourData gadgetBehaviourDatum)
    {
        var lemmingSkillType = gadgetBehaviourDatum.DataChunk.Data1.GetRawValue();
        var lemmingSkill = LemmingSkillConstants.GetEnumValue(lemmingSkillType);

        PackedBytes32 skillData = gadgetBehaviourDatum.DataChunk.Data2;
        int skillCountDelta = (short)skillData.Ushort0;
        int tribeId = skillData.Ushort1;

        return new SkillCountChangeBehaviour(lemmingSkill, tribeId, skillCountDelta)
        {
            GadgetBehaviourName = gadgetBehaviourDatum.GadgetBehaviourName,
            Id = newBehaviourId,
            MaxTriggerCountPerTick = gadgetBehaviourDatum.DataChunk.Data3.GetRawValue()
        };
    }
}
