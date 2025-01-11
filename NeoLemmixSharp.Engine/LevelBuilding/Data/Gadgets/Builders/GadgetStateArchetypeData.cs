using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class GadgetStateArchetypeData
{
    public required IGadgetAction[] OnLemmingEnterActions { get; init; }
    public required IGadgetAction[] OnLemmingPresentActions { get; init; }
    public required IGadgetAction[] OnLemmingExitActions { get; init; }

    public required TriggerType TriggerType { get; init; }
    public required LevelPosition[] TriggerData { get; init; }

    public required GadgetAnimationArchetypeData PrimaryAnimation { get; init; }
    public required int PrimaryAnimationStateTransitionIndex { get; init; }
    public required GadgetAnimationArchetypeData[] SecondaryAnimations { get; init; }

    public LemmingActionSet? AllowedActions { get; init; }
    public StateChangerSet? AllowedStates { get; init; }
    public OrientationSet? AllowedOrientations { get; init; }
    public FacingDirection? AllowedFacingDirection { get; init; }

    public GadgetStateAnimationController GetAnimationController()
    {
        var primaryAnimationBehaviour = PrimaryAnimation.GetAnimationBehaviour();
        var secondaryAnimationBehaviours = GetSecondaryAnimationBehaviours();

        return new GadgetStateAnimationController(
            primaryAnimationBehaviour,
            PrimaryAnimationStateTransitionIndex,
            secondaryAnimationBehaviours);
    }

    private GadgetStateAnimationBehaviour[] GetSecondaryAnimationBehaviours()
    {
        var result = CollectionsHelper.GetArrayForSize<GadgetStateAnimationBehaviour>(SecondaryAnimations.Length);

        for (var i = 0; i < SecondaryAnimations.Length; i++)
        {
            result[i] = SecondaryAnimations[i].GetAnimationBehaviour();
        }

        return result;
    }

    public void Clear()
    {
        PrimaryAnimation.Clear();
        foreach (var secondaryAnimation in SecondaryAnimations)
        {
            secondaryAnimation.Clear();
        }
    }
}

public sealed class GadgetStateArchetypeDataAaa
{
    // region data
    // [hitbox filters]
    // animation controller
}

public sealed class GadgetStateRegionData
{
    // type - empty/rectangle/points
    // [data]
}

public sealed class HitBoxData
{
    // solidity
    // hitbox hint
    // [criteria]
    // [on enter actions]
    // [on present actions]
    // [on leave actions]
}