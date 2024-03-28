using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;
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
        this NeoLemmixGadgetArchetypeData archetypeData,
        SpriteData spriteData)
    {
        return archetypeData.Behaviour switch
        {
            NeoLemmixGadgetBehaviour.None => GetSingleGadgetState(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.Entrance => GetSingleGadgetState(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.Exit => GetSingleGadgetState(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.Water => GetSingleGadgetState(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.Fire => GetSingleGadgetState(archetypeData, spriteData),

            // Deliberately omit the one-way-arrow values

            NeoLemmixGadgetBehaviour.PickupSkill => GetGadgetStatesForTwoStateGadgets(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.LockedExit => ToBeImplemented(NeoLemmixGadgetBehaviour.LockedExit),
            NeoLemmixGadgetBehaviour.UnlockButton => GetGadgetStatesForTwoStateGadgets(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.ForceLeft => GetSingleGadgetState(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.ForceRight => GetSingleGadgetState(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.Trap => GetGadgetStatesForTraps(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.TrapOnce => GetGadgetStatesForTraps(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.Teleporter => ToBeImplemented(NeoLemmixGadgetBehaviour.Teleporter),
            NeoLemmixGadgetBehaviour.Receiver => ToBeImplemented(NeoLemmixGadgetBehaviour.Receiver),
            NeoLemmixGadgetBehaviour.Updraft => GetSingleGadgetState(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.Splitter => ToBeImplemented(NeoLemmixGadgetBehaviour.Splitter),
            NeoLemmixGadgetBehaviour.AntiSplatPad => GetSingleGadgetState(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.SplatPad => GetSingleGadgetState(archetypeData, spriteData),
            NeoLemmixGadgetBehaviour.Background => GetSingleGadgetState(archetypeData, spriteData),

            _ => ThrowUnknownBehaviourException<GadgetStateArchetypeData[]>(archetypeData.Behaviour)
        };
    }

    private static GadgetStateArchetypeData[] GetSingleGadgetState(
        NeoLemmixGadgetArchetypeData archetypeData,
        SpriteData spriteData)
    {
        var emptyActions = Array.Empty<IGadgetAction>();

        var secondaryAnimations = spriteData.NumberOfLayers > 1
            ? new GadgetAnimationArchetypeData[spriteData.NumberOfLayers - 1]
            : Array.Empty<GadgetAnimationArchetypeData>();

        for (var i = 1; i < spriteData.NumberOfLayers; i++)
        {
            secondaryAnimations[i - 1] = new GadgetAnimationArchetypeData
            {
                SpriteWidth = spriteData.SpriteWidth,
                SpriteHeight = spriteData.SpriteHeight,
                Layer = i,
                InitialFrame = 0,
                MinFrame = 0,
                MaxFrame = spriteData.FrameCountsPerLayer[i],
                FrameDelta = 1,
                GadgetStateTransitionIndex = GadgetStateAnimationBehaviour.NoGadgetStateTransition
            };
        }

        var result = new GadgetStateArchetypeData[]
        {
            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerData = archetypeData.ToRectangularTriggerData(),
                PrimaryAnimation = GetPrimaryAnimationArchetypeData(spriteData),
                SecondaryAnimations = secondaryAnimations
            }
        };

        return result;
    }

    private static GadgetStateArchetypeData[] GetGadgetStatesForTraps(
        NeoLemmixGadgetArchetypeData archetypeData,
        SpriteData spriteData)
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

                TriggerData = archetypeData.ToRectangularTriggerData(),
                PrimaryAnimation = GetPrimaryAnimationArchetypeData(spriteData),
                SecondaryAnimations = new GadgetAnimationArchetypeData[]
                {
                }
            },

            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerData = null,
                PrimaryAnimation = GetPrimaryAnimationArchetypeData(spriteData),
                SecondaryAnimations = new GadgetAnimationArchetypeData[]
                {
                }
            },

            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerData = null,
                PrimaryAnimation = GetPrimaryAnimationArchetypeData(spriteData),
                SecondaryAnimations = new GadgetAnimationArchetypeData[]
                {
                }
            }
        };

        return result;
    }

    private static GadgetStateArchetypeData[] GetGadgetStatesForTwoStateGadgets(
        NeoLemmixGadgetArchetypeData archetypeData,
        SpriteData spriteData)
    {
        var emptyActions = Array.Empty<IGadgetAction>();

        var result = new GadgetStateArchetypeData[]
        {
            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerData = archetypeData.ToRectangularTriggerData(),
                PrimaryAnimation = GetPrimaryAnimationArchetypeData(spriteData),
                SecondaryAnimations = new GadgetAnimationArchetypeData[]
                {
                }
            },

            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerData = null,
                PrimaryAnimation = GetPrimaryAnimationArchetypeData(spriteData),
                SecondaryAnimations = new GadgetAnimationArchetypeData[]
                {
                }
            }
        };

        return result;
    }

    private static GadgetAnimationArchetypeData GetPrimaryAnimationArchetypeData(SpriteData spriteData)
    {
        return new GadgetAnimationArchetypeData
        {
            SpriteWidth = spriteData.SpriteWidth,
            SpriteHeight = spriteData.SpriteHeight,
            Layer = 0,
            InitialFrame = 0,
            MinFrame = 0,
            MaxFrame = spriteData.FrameCountsPerLayer[0],
            FrameDelta = 1,
            GadgetStateTransitionIndex = 0
        };
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