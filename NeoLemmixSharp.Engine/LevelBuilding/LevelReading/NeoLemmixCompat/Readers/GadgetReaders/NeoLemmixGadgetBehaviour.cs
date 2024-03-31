using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Diagnostics.CodeAnalysis;

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
        if (archetypeData.Behaviour is NeoLemmixGadgetBehaviour.None or
            NeoLemmixGadgetBehaviour.Entrance or
            NeoLemmixGadgetBehaviour.Exit or
            NeoLemmixGadgetBehaviour.Water or
            NeoLemmixGadgetBehaviour.Fire or
            NeoLemmixGadgetBehaviour.ForceLeft or
            NeoLemmixGadgetBehaviour.ForceRight or
            NeoLemmixGadgetBehaviour.Updraft or
            NeoLemmixGadgetBehaviour.AntiSplatPad or
            NeoLemmixGadgetBehaviour.SplatPad or
            NeoLemmixGadgetBehaviour.Background)
        {
            return GetSingleGadgetState(archetypeData, spriteData);
        }

        if (archetypeData.Behaviour is NeoLemmixGadgetBehaviour.PickupSkill or
            NeoLemmixGadgetBehaviour.UnlockButton or
            NeoLemmixGadgetBehaviour.Splitter)
        {
            return GetGadgetStatesForTwoStateGadgets(archetypeData, spriteData);
        }

        if (archetypeData.Behaviour is NeoLemmixGadgetBehaviour.Trap or
            NeoLemmixGadgetBehaviour.TrapOnce)
        {
            return GetGadgetStatesForTraps(archetypeData, spriteData);
        }

        return ToBeImplemented(archetypeData.Behaviour);
    }

    private static GadgetStateArchetypeData[] GetSingleGadgetState(
        NeoLemmixGadgetArchetypeData archetypeData,
        SpriteData spriteData)
    {
        var emptyActions = Array.Empty<IGadgetAction>();

        GadgetAnimationArchetypeData? secondaryAnimation;
        if (spriteData.NumberOfLayers == 1)
        {
            secondaryAnimation = null;
        }
        else
        {
            secondaryAnimation = new GadgetAnimationArchetypeData
            {
                SpriteWidth = spriteData.SpriteWidth,
                SpriteHeight = spriteData.SpriteHeight,
                Layer = 1,
                InitialFrame = 0,
                MinFrame = 0,
                MaxFrame = spriteData.FrameCountsPerLayer[1]
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
                PrimaryAnimation = new GadgetAnimationArchetypeData
                {
                    SpriteWidth = spriteData.SpriteWidth,
                    SpriteHeight = spriteData.SpriteHeight,
                    Layer = 0,
                    InitialFrame = 0,
                    MinFrame = 0,
                    MaxFrame = spriteData.FrameCountsPerLayer[0]
                },
                PrimaryAnimationStateTransitionIndex = GadgetStateAnimationController.NoGadgetStateTransition,
                SecondaryAnimation = secondaryAnimation,
                SecondaryAnimationAction = GadgetSecondaryAnimationAction.Play
            }
        };

        return result;
    }

    private static GadgetStateArchetypeData[] GetGadgetStatesForTraps(
        NeoLemmixGadgetArchetypeData archetypeData,
        SpriteData spriteData)
    {
        var emptyActions = Array.Empty<IGadgetAction>();

        GadgetAnimationArchetypeData? secondaryAnimation;
        AnimationData? secondaryAnimationData;
        if (archetypeData.AnimationData.Count == 0)
        {
            secondaryAnimation = null;
            secondaryAnimationData = null;
        }
        else
        {
            secondaryAnimation = new GadgetAnimationArchetypeData
            {
                SpriteWidth = spriteData.SpriteWidth,
                SpriteHeight = spriteData.SpriteHeight,
                Layer = 1,
                InitialFrame = 0,
                MinFrame = 0,
                MaxFrame = spriteData.FrameCountsPerLayer[1]
            };
            secondaryAnimationData = archetypeData.AnimationData[0];
        }

        var idleState = new GadgetStateArchetypeData
        {
            OnLemmingEnterActions = emptyActions,
            OnLemmingPresentActions = emptyActions,
            OnLemmingExitActions = emptyActions,

            TriggerData = archetypeData.ToRectangularTriggerData(),
            PrimaryAnimation = new GadgetAnimationArchetypeData
            {
                SpriteWidth = spriteData.SpriteWidth,
                SpriteHeight = spriteData.SpriteHeight,
                Layer = 0,
                InitialFrame = 0,
                MinFrame = 0,
                MaxFrame = 1
            },
            PrimaryAnimationStateTransitionIndex = GadgetStateAnimationController.NoGadgetStateTransition, // Idle loops to Idle
            SecondaryAnimation = GetSecondaryAnimationArchetypeDataForState(NeoLemmixGadgetStateType.Idle),
            SecondaryAnimationAction = GetGadgetSecondaryAnimationActionForState(NeoLemmixGadgetStateType.Idle)
        };

        var activeState = new GadgetStateArchetypeData
        {
            OnLemmingEnterActions = emptyActions,
            OnLemmingPresentActions = emptyActions,
            OnLemmingExitActions = emptyActions,

            TriggerData = null,
            PrimaryAnimation = new GadgetAnimationArchetypeData
            {
                SpriteWidth = spriteData.SpriteWidth,
                SpriteHeight = spriteData.SpriteHeight,
                Layer = 0,
                InitialFrame = 0,
                MinFrame = 0,
                MaxFrame = spriteData.FrameCountsPerLayer[0]
            },
            PrimaryAnimationStateTransitionIndex = 0, // Transition to Idle
            SecondaryAnimation = GetSecondaryAnimationArchetypeDataForState(NeoLemmixGadgetStateType.Active),
            SecondaryAnimationAction = GetGadgetSecondaryAnimationActionForState(NeoLemmixGadgetStateType.Active)
        };

        // Disabled
        var disabledState = new GadgetStateArchetypeData
        {
            OnLemmingEnterActions = emptyActions,
            OnLemmingPresentActions = emptyActions,
            OnLemmingExitActions = emptyActions,

            TriggerData = null,
            PrimaryAnimation = new GadgetAnimationArchetypeData
            {
                SpriteWidth = spriteData.SpriteWidth,
                SpriteHeight = spriteData.SpriteHeight,
                Layer = 0,
                InitialFrame = 0,
                MinFrame = 0,
                MaxFrame = 1
            },
            PrimaryAnimationStateTransitionIndex = GadgetStateAnimationController.NoGadgetStateTransition, // Stay Disabled
            SecondaryAnimation = GetSecondaryAnimationArchetypeDataForState(NeoLemmixGadgetStateType.Disabled),
            SecondaryAnimationAction = GetGadgetSecondaryAnimationActionForState(NeoLemmixGadgetStateType.Disabled)

        };
        // Three states: idle, active, disabled
        var result = new[]
        {
            idleState,
            activeState,
            disabledState
        };

        return result;

        GadgetAnimationArchetypeData? GetSecondaryAnimationArchetypeDataForState(NeoLemmixGadgetStateType state)
        {
            if (secondaryAnimation is null)
                return null;

            var animationTriggerData = GetAnimationTriggerDataForState(state);
            if (animationTriggerData is null)
                return secondaryAnimation;

            return animationTriggerData.Hide ? null : secondaryAnimation;
        }

        GadgetSecondaryAnimationAction GetGadgetSecondaryAnimationActionForState(NeoLemmixGadgetStateType state)
        {
            if (secondaryAnimation is null)
                return GadgetSecondaryAnimationAction.Play;

            var animationTriggerData = GetAnimationTriggerDataForState(state);
            return animationTriggerData?.AnimationAction ?? GadgetSecondaryAnimationAction.Play;
        }

        AnimationTriggerData? GetAnimationTriggerDataForState(NeoLemmixGadgetStateType state)
        {
            if (secondaryAnimationData is null)
                return null;

            foreach (var animationTriggerData in secondaryAnimationData.TriggerData)
            {
                if (animationTriggerData.StateType == state)
                    return animationTriggerData;
            }

            return null;
        }
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
                PrimaryAnimation = new GadgetAnimationArchetypeData
                {
                    SpriteWidth = spriteData.SpriteWidth,
                    SpriteHeight = spriteData.SpriteHeight,
                    Layer = 0,
                    InitialFrame = 0,
                    MinFrame = 0,
                    MaxFrame = 1
                },
                PrimaryAnimationStateTransitionIndex = 1,
                SecondaryAnimation = null,
                SecondaryAnimationAction = GadgetSecondaryAnimationAction.Play
            },

            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerData = archetypeData.Behaviour == NeoLemmixGadgetBehaviour.Splitter
                    ? archetypeData.ToRectangularTriggerData()
                    : null,
                PrimaryAnimation = new GadgetAnimationArchetypeData
                {
                    SpriteWidth = spriteData.SpriteWidth,
                    SpriteHeight = spriteData.SpriteHeight,
                    Layer = 0,
                    InitialFrame = 1,
                    MinFrame = 1,
                    MaxFrame = 2
                },
                PrimaryAnimationStateTransitionIndex = 0,
                SecondaryAnimation = null,
                SecondaryAnimationAction = GadgetSecondaryAnimationAction.Play
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