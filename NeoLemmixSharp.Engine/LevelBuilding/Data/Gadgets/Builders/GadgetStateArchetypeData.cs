using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class GadgetStateArchetypeData
{
    public required IGadgetAction[] OnLemmingEnterActions { get; init; }
    public required IGadgetAction[] OnLemmingPresentActions { get; init; }
    public required IGadgetAction[] OnLemmingExitActions { get; init; }

    public required RectangularTriggerData? TriggerData { get; init; }

    public required GadgetAnimationArchetypeData PrimaryAnimation { get; init; }
    public required GadgetAnimationArchetypeData[] SecondaryAnimations { get; init; }

    public SimpleSet<LemmingAction>? AllowedActions { get; init; }
    public SimpleSet<ILemmingStateChanger>? AllowedStates { get; init; }
    public SimpleSet<Orientation>? AllowedOrientations { get; init; }
    public FacingDirection? AllowedFacingDirection { get; init; }

    public GadgetStateAnimationBehaviour GetPrimaryAnimationBehaviour()
    {
        return ToAnimationBehaviour(PrimaryAnimation);
    }

    public GadgetStateAnimationBehaviour[] GetSecondaryAnimationBehaviours()
    {
        if (SecondaryAnimations.Length == 0)
            return Array.Empty<GadgetStateAnimationBehaviour>();

        var result = new GadgetStateAnimationBehaviour[SecondaryAnimations.Length];

        for (var index = 0; index < SecondaryAnimations.Length; index++)
        {
            result[index] = ToAnimationBehaviour(SecondaryAnimations[index]);
        }

        return result;
    }

    private static GadgetStateAnimationBehaviour ToAnimationBehaviour(GadgetAnimationArchetypeData archetypeData)
    {
        return new GadgetStateAnimationBehaviour(
            archetypeData.SpriteWidth,
            archetypeData.SpriteHeight,
            archetypeData.Layer,
            archetypeData.InitialFrame,
            archetypeData.MinFrame,
            archetypeData.MaxFrame,
            archetypeData.FrameDelta,
            archetypeData.GadgetStateTransitionIndex);
    }
}

public sealed class GadgetAnimationArchetypeData
{
    public required int SpriteWidth { get; init; }
    public required int SpriteHeight { get; init; }
    public required int Layer { get; init; }
    public required int InitialFrame { get; init; }
    public required int MinFrame { get; init; }
    public required int MaxFrame { get; init; }
    public required int FrameDelta { get; init; }
    public required int GadgetStateTransitionIndex { get; init; }
}