using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using NeoLemmixSharp.IO.Util;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2;

public readonly ref struct GadgetTriggerBuilder
{
    private readonly GadgetIdentifier _gadgetIdentifier;
    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public GadgetTriggerBuilder(
        GadgetIdentifier gadgetIdentifier,
        List<GadgetTrigger> gadgetTriggers,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _gadgetIdentifier = gadgetIdentifier;
        _gadgetTriggers = gadgetTriggers;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public GadgetTrigger[] BuildGadgetTriggers(
        IGadgetStateArchetypeData gadgetStateArchetypeData,
        IGadgetStateInstanceData gadgetStateInstanceData)
    {
        var gadgetBehaviourBuilder = new GadgetBehaviourBuilder(_gadgetIdentifier, _gadgetBehaviours);
        var behaviourLookup = gadgetBehaviourBuilder.BuildBehaviourLookup(gadgetStateArchetypeData.InnateBehaviours, gadgetStateInstanceData.CustomBehaviours);

        var allTriggerBehaviourLinks = GetGadgetTriggerBehaviourLinks(gadgetStateArchetypeData.TriggerBehaviourLinks, gadgetStateInstanceData.CustomTriggerBehaviourLinks);

        var result = Helpers.GetArrayForSize<GadgetTrigger>(gadgetStateArchetypeData.InnateTriggers.Length + gadgetStateInstanceData.CustomTriggers.Length);
        var i = 0;

        for (var a = 0; a < gadgetStateArchetypeData.InnateTriggers.Length; a++)
        {
            ref readonly var gadgetTriggerDatum = ref gadgetStateArchetypeData.InnateTriggers[a];
            var newGadgetTrigger = BuildTrigger(in gadgetTriggerDatum, behaviourLookup, allTriggerBehaviourLinks);
            result[i++] = newGadgetTrigger;
        }

        for (var a = 0; a < gadgetStateInstanceData.CustomTriggers.Length; a++)
        {
            ref readonly var gadgetTriggerDatum = ref gadgetStateInstanceData.CustomTriggers[a];
            var newGadgetTrigger = BuildTrigger(in gadgetTriggerDatum, behaviourLookup, allTriggerBehaviourLinks);
            result[i++] = newGadgetTrigger;
        }

        return result;
    }

    private static HashSet<GadgetTriggerBehaviourLink> GetGadgetTriggerBehaviourLinks(
        ReadOnlySpan<GadgetTriggerBehaviourLink> innateTriggerBehaviourLinks,
        ReadOnlySpan<GadgetTriggerBehaviourLink> customTriggerBehaviourLinks)
    {
        var result = new HashSet<GadgetTriggerBehaviourLink>(innateTriggerBehaviourLinks.Length + customTriggerBehaviourLinks.Length);

        foreach (var link in innateTriggerBehaviourLinks)
        {
            result.Add(link);
        }
        foreach (var link in customTriggerBehaviourLinks)
        {
            result.Add(link);
        }

        return result;
    }

    private GadgetTrigger BuildTrigger(
        in GadgetTriggerData gadgetTriggerDatum,
        Dictionary<GadgetBehaviourName, GadgetBehaviour> behaviourLookup,
        HashSet<GadgetTriggerBehaviourLink> allTriggerBehaviourLinks)
    {
        var newTriggerId = _gadgetTriggers.Count;
        var behavioursForTrigger = GetBehavioursMentionedByTrigger(behaviourLookup, allTriggerBehaviourLinks, gadgetTriggerDatum.GadgetTriggerName);

        GadgetTrigger result = gadgetTriggerDatum.GadgerTriggerType switch
        {
            GadgetTriggerType.AlwaysTrue => BuildAlwaysTrueGadgetTrigger(in gadgetTriggerDatum, newTriggerId, behavioursForTrigger),
            GadgetTriggerType.GadgetLinkTrigger => BuildGadgetLinkTrigger(in gadgetTriggerDatum, newTriggerId, behavioursForTrigger),
            GadgetTriggerType.GadgetPositionTrigger => BuildGadgetPositionTrigger(in gadgetTriggerDatum, newTriggerId, behavioursForTrigger),
            GadgetTriggerType.GadgetAnimationFinished => BuildGadgetAnimationFinishedTrigger(in gadgetTriggerDatum, newTriggerId, behavioursForTrigger),
            GadgetTriggerType.LemmingHitBoxTrigger => throw new InvalidOperationException("Cannot build LemmingHitBoxTrigger here!"),
            //GadgetTriggerType.GlobalLevelTimerTrigger => BuildGlobalLevelTimerTrigger(in gadgetTriggerDatum, newTriggerId, behavioursForTrigger),

            _ => Helpers.ThrowUnknownEnumValueException<GadgetTriggerType, GadgetTrigger>(gadgetTriggerDatum.GadgerTriggerType)
        };

        _gadgetTriggers.Add(result);

        return result;
    }

    private static AlwaysTrueTrigger BuildAlwaysTrueGadgetTrigger(
        in GadgetTriggerData gadgetTriggerDatum,
        int newTriggerId,
        GadgetBehaviour[] behaviours)
    {
        return new AlwaysTrueTrigger()
        {
            TriggerName = gadgetTriggerDatum.GadgetTriggerName,
            Id = newTriggerId,
            Behaviours = behaviours
        };
    }

    private static GadgetLinkTrigger BuildGadgetLinkTrigger(
        in GadgetTriggerData gadgetTriggerDatum,
        int newTriggerId,
        GadgetBehaviour[] behaviours)
    {
        return new GadgetLinkTrigger()
        {
            TriggerName = gadgetTriggerDatum.GadgetTriggerName,
            Id = newTriggerId,
            Behaviours = behaviours
        };
    }

    private static GadgetPositionTrigger BuildGadgetPositionTrigger(
        in GadgetTriggerData gadgetTriggerDatum,
        int newTriggerId,
        GadgetBehaviour[] behaviours)
    {
        var point = ReadWriteHelpers.DecodePoint(gadgetTriggerDatum.DataChunk.Data1);
        uint miscData = (uint)gadgetTriggerDatum.DataChunk.Data2;
        uint t = miscData >>> 0;
        var requireX = (t & 1) != 0;
        t = (miscData >>> 1) & 3;
        var comparisonX = ComparisonTypeHelpers.GetEnumValue(t);
        t = miscData >>> 4;
        var requireY = (t & 1) != 0;
        t = (miscData >>> 5) & 3;
        var comparisonY = ComparisonTypeHelpers.GetEnumValue(t);

        return new GadgetPositionTrigger(
            point.X,
            point.Y,
            comparisonX,
            comparisonY,
            requireX,
            requireY)
        {
            TriggerName = gadgetTriggerDatum.GadgetTriggerName,
            Id = newTriggerId,
            Behaviours = behaviours
        };
    }

    private static GadgetTrigger BuildGadgetAnimationFinishedTrigger(
        in GadgetTriggerData gadgetTriggerDatum,
        int newTriggerId,
        GadgetBehaviour[] behaviours)
    {
        throw new NotImplementedException();
    }
    /*
    private static LevelTimerTrigger BuildGlobalLevelTimerTrigger(
        in GadgetTriggerData gadgetTriggerDatum,
        int newTriggerId,
        GadgetBehaviour[] behaviours)
    {
        if (behaviours.Length != 1 ||
            behaviours[0] is not OutputSignalBehaviour outputSignalBehaviour)
            throw new InvalidOperationException("Expected ONE OutputSignalBehaviour!");

        var rawData = (uint)gadgetTriggerDatum.Data1;
        var rawLevelTimerObservationTypeId = rawData & 0xff;
        var rawComparisonTypeId = (rawData >>> 8) & 0xff;
        var levelTimerObservationType = LevelTimerObservationTypeHelpers.GetEnumValue(rawLevelTimerObservationTypeId);
        var comparisonType = ComparisonTypeHelpers.GetEnumValue(rawComparisonTypeId);

        var requiredValue = gadgetTriggerDatum.Data2;

        var levelTimerTriggerParameters = new LevelTimerTriggerParameters(levelTimerObservationType, comparisonType, requiredValue);

        return new LevelTimerTrigger(outputSignalBehaviour, levelTimerTriggerParameters)
        {
            TriggerName = gadgetTriggerDatum.GadgetTriggerName,
            Id = newTriggerId,
            Behaviours = []
        };
    }
    */
    private static GadgetBehaviour[] GetBehavioursMentionedByTrigger(
        Dictionary<GadgetBehaviourName, GadgetBehaviour> behaviourLookup,
        HashSet<GadgetTriggerBehaviourLink> allTriggerBehaviourLinks,
        GadgetTriggerName gadgetTriggerName)
    {
        var resultLength = GetNumberOfBehavioursMentionedByTrigger(allTriggerBehaviourLinks, gadgetTriggerName);
        var result = Helpers.GetArrayForSize<GadgetBehaviour>(resultLength);

        var i = 0;
        foreach (var triggerBehaviourLink in allTriggerBehaviourLinks)
        {
            if (gadgetTriggerName.Equals(triggerBehaviourLink.GadgetTriggerName))
            {
                var behaviour = behaviourLookup[triggerBehaviourLink.GadgetBehaviourName];
                result[i++] = behaviour;
            }
        }

        Debug.Assert(i == result.Length);

        return result;
    }

    private static int GetNumberOfBehavioursMentionedByTrigger(
        HashSet<GadgetTriggerBehaviourLink> allTriggerBehaviourLinks,
        GadgetTriggerName gadgetTriggerName)
    {
        var i = 0;
        foreach (var triggerBehaviourLink in allTriggerBehaviourLinks)
        {
            if (gadgetTriggerName.Equals(triggerBehaviourLink.GadgetTriggerName))
                i++;
        }

        return i;
    }
}
