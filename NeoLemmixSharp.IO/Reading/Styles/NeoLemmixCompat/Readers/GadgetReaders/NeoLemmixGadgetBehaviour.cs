using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.GadgetReaders;

internal enum NeoLemmixGadgetBehaviour
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

internal static class NeoLemmixGadgetBehaviourExtensions
{
    public static bool IsOneWayArrows(this NeoLemmixGadgetBehaviour behaviour)
    {
        return behaviour is
            NeoLemmixGadgetBehaviour.OneWayRight or
            NeoLemmixGadgetBehaviour.OneWayUp or
            NeoLemmixGadgetBehaviour.OneWayLeft or
            NeoLemmixGadgetBehaviour.OneWayDown;
    }
    /*
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
        SpriteArchetypeData spriteData)
    {
          if (archetypeData.Behaviour is
              NeoLemmixGadgetBehaviour.None or
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
              return GetSingleGadgetState(archetypeData, spriteData);

          if (archetypeData.Behaviour is
              NeoLemmixGadgetBehaviour.PickupSkill or
              NeoLemmixGadgetBehaviour.UnlockButton or
              NeoLemmixGadgetBehaviour.Splitter)
              return GetGadgetStatesForTwoStateGadgets(archetypeData, spriteData);

          if (archetypeData.Behaviour is
              NeoLemmixGadgetBehaviour.Trap or
              NeoLemmixGadgetBehaviour.TrapOnce)
              return GetGadgetStatesForTraps(archetypeData, spriteData);
          
        return ToBeImplemented(archetypeData.Behaviour);
    }
    
    private static GadgetStateArchetypeData[] GetSingleGadgetState(
        NeoLemmixGadgetArchetypeData archetypeData,
        SpriteData spriteData)
    {
        var emptyActions = Array.Empty<IGadgetAction>();

        GadgetAnimationArchetypeData[] secondaryAnimations;
        if (spriteData.NumberOfLayers == 1)
        {
            secondaryAnimations = [];
        }
        else
        {
            secondaryAnimations =
            [
                new GadgetAnimationArchetypeData
                {
                    SpriteWidth = spriteData.SpriteWidth,
                    SpriteHeight = spriteData.SpriteHeight,
                    Layer = 1,
                    InitialFrame = 0,
                    MinFrame = 0,
                    MaxFrame = spriteData.FrameCountsPerLayer[1],
                    SecondaryAnimationAction = GadgetSecondaryAnimationAction.Play
                }
            ];
        }

        var result = new GadgetStateArchetypeData[]
        {
            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerType = HitBoxType.Rectangular,
                TriggerData = archetypeData.ToRectangularTriggerData(),
                PrimaryAnimation = new GadgetAnimationArchetypeData
                {
                    SpriteWidth = spriteData.SpriteWidth,
                    SpriteHeight = spriteData.SpriteHeight,
                    Layer = 0,
                    InitialFrame = 0,
                    MinFrame = 0,
                    MaxFrame = spriteData.FrameCountsPerLayer[0],
                    SecondaryAnimationAction = GadgetSecondaryAnimationAction.Play
                },
                PrimaryAnimationStateTransitionIndex = GadgetStateAnimationController.NoGadgetStateTransition,
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

        GadgetAnimationArchetypeData[] secondaryAnimations;
        AnimationData? secondaryAnimationData;
        if (archetypeData.AnimationData.Count == 0)
        {
            secondaryAnimations = [];
            secondaryAnimationData = null;
        }
        else
        {
            secondaryAnimations =
            [
                new GadgetAnimationArchetypeData
                {
                    SpriteWidth = spriteData.SpriteWidth,
                    SpriteHeight = spriteData.SpriteHeight,
                    Layer = 1,
                    InitialFrame = 0,
                    MinFrame = 0,
                    MaxFrame = spriteData.FrameCountsPerLayer[1],
                    SecondaryAnimationAction = GadgetSecondaryAnimationAction.Play
                }
            ];
            secondaryAnimationData = archetypeData.AnimationData[0];
        }

            var idleState = new GadgetStateArchetypeData
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerType = HitBoxType.Rectangular,
                TriggerData = archetypeData.ToRectangularTriggerData(),
                PrimaryAnimation = new GadgetAnimationArchetypeData
                {
                    SpriteWidth = spriteData.SpriteWidth,
                    SpriteHeight = spriteData.SpriteHeight,
                    Layer = 0,
                    InitialFrame = 0,
                    MinFrame = 0,
                    MaxFrame = 1,
                    SecondaryAnimationAction = GadgetSecondaryAnimationAction.Play
                },
                PrimaryAnimationStateTransitionIndex = GadgetStateAnimationController.NoGadgetStateTransition, // Idle loops to Idle
                SecondaryAnimations = GetSecondaryAnimationArchetypeDataForState(NeoLemmixGadgetStateType.Idle)
            };

            var activeState = new GadgetStateArchetypeData
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerType = HitBoxType.Rectangular,
                TriggerData = [],
                PrimaryAnimation = new GadgetAnimationArchetypeData
                {
                    SpriteWidth = spriteData.SpriteWidth,
                    SpriteHeight = spriteData.SpriteHeight,
                    Layer = 0,
                    InitialFrame = 0,
                    MinFrame = 0,
                    MaxFrame = spriteData.FrameCountsPerLayer[0],
                    SecondaryAnimationAction = GadgetSecondaryAnimationAction.Play
                },
                PrimaryAnimationStateTransitionIndex = 0, // Transition to Idle
                SecondaryAnimations = GetSecondaryAnimationArchetypeDataForState(NeoLemmixGadgetStateType.Active),
            };

            var disabledState = new GadgetStateArchetypeData
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerType = HitBoxType.Rectangular,
                TriggerData = [],
                PrimaryAnimation = new GadgetAnimationArchetypeData
                {
                    SpriteWidth = spriteData.SpriteWidth,
                    SpriteHeight = spriteData.SpriteHeight,
                    Layer = 0,
                    InitialFrame = 0,
                    MinFrame = 0,
                    MaxFrame = 1,
                    SecondaryAnimationAction = GadgetSecondaryAnimationAction.Play
                },
                PrimaryAnimationStateTransitionIndex = GadgetStateAnimationController.NoGadgetStateTransition, // Stay Disabled
                SecondaryAnimations = GetSecondaryAnimationArchetypeDataForState(NeoLemmixGadgetStateType.Disabled),

            };

        var result = new GadgetStateArchetypeData[]
        {
          //  idleState,
          //  activeState,
          //  disabledState
        };

        return result;

        GadgetAnimationArchetypeData[] GetSecondaryAnimationArchetypeDataForState(NeoLemmixGadgetStateType state)
        {
            if (secondaryAnimations.Length == 0)
                return secondaryAnimations;

            var animationTriggerData = GetAnimationTriggerDataForState(state);
            if (animationTriggerData is null)
                return secondaryAnimations;

            return animationTriggerData.Hide ? [] : secondaryAnimations;
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

                TriggerType = HitBoxType.Rectangular,
                TriggerData = archetypeData.ToRectangularTriggerData(),
                PrimaryAnimation = new GadgetAnimationArchetypeData
                {
                    SpriteWidth = spriteData.SpriteWidth,
                    SpriteHeight = spriteData.SpriteHeight,
                    Layer = 0,
                    InitialFrame = 0,
                    MinFrame = 0,
                    MaxFrame = 1,
                    SecondaryAnimationAction = GadgetSecondaryAnimationAction.Play
                },
                PrimaryAnimationStateTransitionIndex = 1,
                SecondaryAnimations = []
            },

            new()
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                TriggerType = HitBoxType.Rectangular,
                TriggerData = archetypeData.Behaviour == NeoLemmixGadgetBehaviour.Splitter
                    ? archetypeData.ToRectangularTriggerData()
                    : [],
                PrimaryAnimation = new GadgetAnimationArchetypeData
                {
                    SpriteWidth = spriteData.SpriteWidth,
                    SpriteHeight = spriteData.SpriteHeight,
                    Layer = 0,
                    InitialFrame = 1,
                    MinFrame = 1,
                    MaxFrame = 2,
                    SecondaryAnimationAction = GadgetSecondaryAnimationAction.Play
                },
                PrimaryAnimationStateTransitionIndex = 0,
                SecondaryAnimations = []
            }
        };

        return result;
    }
*/
    [DoesNotReturn]
    private static HitBoxGadgetStateArchetypeData[] ToBeImplemented(NeoLemmixGadgetBehaviour behaviour)
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