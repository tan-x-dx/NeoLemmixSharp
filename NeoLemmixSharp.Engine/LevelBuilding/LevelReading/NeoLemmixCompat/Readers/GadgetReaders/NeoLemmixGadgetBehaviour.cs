using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;

public enum NeoLemmixGadgetBehaviour
{
    None,
    Entrance,
    Exit,
    Water,
    Fire,
    OneWayRight,
    OneWayUp,
    OneWayLeft,
    OneWayDown,
    PickupSkill,
    LockedExit,
    UnlockButton,
    ForceLeft,
    ForceRight,
    Trap,
    TrapOnce,
    Teleporter,
    Receiver,
    Updraft,
    Splitter,
    AntiSplatPad,
    SplatPad,
    Background
}

public static class NeoLemmixGadgetBehaviourExtensions
{
    public static GadgetBehaviour ToGadgetBehaviour(
        this NeoLemmixGadgetBehaviour neoLemmixGadgetBehaviour)
    {
        return neoLemmixGadgetBehaviour switch
        {
            NeoLemmixGadgetBehaviour.None => GenericGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.Entrance => GenericGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.Exit => GenericGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.Water => WaterGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.Fire => FireGadgetBehaviour.Instance,

            // Deliberately omit the one-way-arrow values

            NeoLemmixGadgetBehaviour.PickupSkill => GenericGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.LockedExit => GenericGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.UnlockButton => GenericGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.ForceLeft => GenericGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.ForceRight => GenericGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.Trap => TinkerableGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.TrapOnce => TinkerableGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.Teleporter => GenericGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.Receiver => GenericGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.Updraft => UpdraftGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.Splitter => GenericGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.AntiSplatPad => NoSplatGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.SplatPad => SplatGadgetBehaviour.Instance,
            NeoLemmixGadgetBehaviour.Background => GenericGadgetBehaviour.Instance,

            _ => ThrowUnknownBehaviourException<GadgetBehaviour>(neoLemmixGadgetBehaviour)
        };
    }

    public static GadgetStateArchetypeData[] GetGadgetStates(
        this NeoLemmixGadgetArchetypeData archetypeData)
    {
        return archetypeData.Behaviour switch
        {
            NeoLemmixGadgetBehaviour.None => GetSingleGadgetState(archetypeData),
            NeoLemmixGadgetBehaviour.Entrance => GetSingleGadgetState(archetypeData),
            NeoLemmixGadgetBehaviour.Exit => GetSingleGadgetState(archetypeData),
            NeoLemmixGadgetBehaviour.Water => GetSingleGadgetState(archetypeData),
            NeoLemmixGadgetBehaviour.Fire => GetSingleGadgetState(archetypeData),

            // Deliberately omit the one-way-arrow values

            NeoLemmixGadgetBehaviour.PickupSkill => GetGadgetStatesForTwoStateGadgets(archetypeData),
            NeoLemmixGadgetBehaviour.LockedExit => ToBeImplemented(NeoLemmixGadgetBehaviour.LockedExit),
            NeoLemmixGadgetBehaviour.UnlockButton => GetGadgetStatesForTwoStateGadgets(archetypeData),
            NeoLemmixGadgetBehaviour.ForceLeft => GetSingleGadgetState(archetypeData),
            NeoLemmixGadgetBehaviour.ForceRight => GetSingleGadgetState(archetypeData),
            NeoLemmixGadgetBehaviour.Trap => GetGadgetStatesForTraps(archetypeData),
            NeoLemmixGadgetBehaviour.TrapOnce => GetGadgetStatesForTraps(archetypeData),
            NeoLemmixGadgetBehaviour.Teleporter => ToBeImplemented(NeoLemmixGadgetBehaviour.Teleporter),
            NeoLemmixGadgetBehaviour.Receiver => ToBeImplemented(NeoLemmixGadgetBehaviour.Receiver),
            NeoLemmixGadgetBehaviour.Updraft => GetSingleGadgetState(archetypeData),
            NeoLemmixGadgetBehaviour.Splitter => ToBeImplemented(NeoLemmixGadgetBehaviour.Splitter),
            NeoLemmixGadgetBehaviour.AntiSplatPad => GetSingleGadgetState(archetypeData),
            NeoLemmixGadgetBehaviour.SplatPad => GetSingleGadgetState(archetypeData),
            NeoLemmixGadgetBehaviour.Background => GetSingleGadgetState(archetypeData),

            _ => ThrowUnknownBehaviourException<GadgetStateArchetypeData[]>(archetypeData.Behaviour)
        };
    }

    private static GadgetStateArchetypeData[] GetSingleGadgetState(NeoLemmixGadgetArchetypeData archetypeData)
    {
        var emptyActions = Array.Empty<IGadgetAction>();

        var result = new GadgetStateArchetypeData[]
        {
            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerData = new RectangularTriggerData
                {
                    TriggerX = archetypeData.TriggerX,
                    TriggerY = archetypeData.TriggerY,
                    TriggerWidth = archetypeData.TriggerWidth,
                    TriggerHeight = archetypeData.TriggerHeight
                }
            }
        };

        return result;
    }

    private static GadgetStateArchetypeData[] GetGadgetStatesForTraps(NeoLemmixGadgetArchetypeData archetypeData)
    {
        var emptyActions = Array.Empty<IGadgetAction>();

        var animationDataSpan = CollectionsMarshal.AsSpan(archetypeData.AnimationData);

        // Three states: idle, active, disabled
        var result = new GadgetStateArchetypeData[]
        {
            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerData = new RectangularTriggerData
                {
                    TriggerX = archetypeData.TriggerX,
                    TriggerY = archetypeData.TriggerY,
                    TriggerWidth = archetypeData.TriggerWidth,
                    TriggerHeight = archetypeData.TriggerHeight
                }
            },

            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerData = null
            },

            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerData = null
            }
        };

        return result;
    }

    private static GadgetStateArchetypeData[] GetGadgetStatesForTwoStateGadgets(NeoLemmixGadgetArchetypeData archetypeData)
    {
        var emptyActions = Array.Empty<IGadgetAction>();

        var result = new GadgetStateArchetypeData[]
        {
            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerData = new RectangularTriggerData
                {
                    TriggerX = archetypeData.TriggerX,
                    TriggerY = archetypeData.TriggerY,
                    TriggerWidth = archetypeData.TriggerWidth,
                    TriggerHeight = archetypeData.TriggerHeight
                }
            },

            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerData = null
            }
        };

        return result;
    }

    private static GadgetStateArchetypeData[] ToBeImplemented(NeoLemmixGadgetBehaviour behaviour)
    {
        throw new NotImplementedException($"Need to implement this behaviour: {behaviour}");
    }

    [DoesNotReturn]
    private static T ThrowUnknownBehaviourException<T>(NeoLemmixGadgetBehaviour neoLemmixGadgetBehaviour)
    {
        throw new ArgumentOutOfRangeException(
            nameof(neoLemmixGadgetBehaviour),
            neoLemmixGadgetBehaviour,
            "Unknown NeoLemmix gadget behaviour");
    }
}