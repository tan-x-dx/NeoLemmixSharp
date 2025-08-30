using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
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

        var allTriggerBehaviourLinks = Helpers.CombineSpans(gadgetStateArchetypeData.TriggerBehaviourLinks, gadgetStateInstanceData.CustomTriggerBehaviourLinks);

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

    private GadgetTrigger BuildTrigger(
        in GadgetTriggerData gadgetTriggerDatum,
        Dictionary<GadgetBehaviourName, GadgetBehaviour> behaviourLookup,
        ReadOnlySpan<GadgetTriggerBehaviourLink> allTriggerBehaviourLinks)
    {
        var newTriggerId = _gadgetTriggers.Count;
        var behaviours = GetBehaviours(behaviourLookup, allTriggerBehaviourLinks, gadgetTriggerDatum.GadgetTriggerName);

        GadgetTrigger result = gadgetTriggerDatum.GadgerTriggerType switch
        {
            GadgetTriggerType.None => throw new InvalidOperationException("Invalid Trigger Type!"),

            GadgetTriggerType.AlwaysTrue => BuildAlwaysTrueGadgetTrigger(in gadgetTriggerDatum, newTriggerId, behaviours),
            GadgetTriggerType.GadgetLinkTrigger => BuildGadgetLinkTrigger(in gadgetTriggerDatum, newTriggerId, behaviours),
            GadgetTriggerType.GadgetAnimationFinished => BuildGadgetAnimationFinishedTrigger(in gadgetTriggerDatum, newTriggerId, behaviours),
            GadgetTriggerType.LemmingHitBoxTrigger => throw new InvalidOperationException("Cannot build LemmingHitBoxTrigger here!"),

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
            GadgetBehaviours = behaviours,
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
            GadgetBehaviours = behaviours,
        };
    }

    private static GadgetTrigger BuildGadgetAnimationFinishedTrigger(
        in GadgetTriggerData gadgetTriggerDatum,
        int newTriggerId,
        GadgetBehaviour[] behaviours)
    {
        throw new NotImplementedException();
    }

    private static GadgetBehaviour[] GetBehaviours(
        Dictionary<GadgetBehaviourName, GadgetBehaviour> behaviourLookup,
        ReadOnlySpan<GadgetTriggerBehaviourLink> allTriggerBehaviourLinks,
        GadgetTriggerName gadgetTriggerName)
    {
        var resultLength = GetNumberOfBehaviours(allTriggerBehaviourLinks, gadgetTriggerName);
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

    private static int GetNumberOfBehaviours(
        ReadOnlySpan<GadgetTriggerBehaviourLink> allTriggerBehaviourLinks,
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
