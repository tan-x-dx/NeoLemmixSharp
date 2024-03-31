using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
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
    public required int PrimaryAnimationStateTransitionIndex { get; init; }
    public required GadgetAnimationArchetypeData? SecondaryAnimation { get; init; }
    public required GadgetSecondaryAnimationAction SecondaryAnimationAction { get; init; }

    public SimpleSet<LemmingAction>? AllowedActions { get; init; }
    public SimpleSet<ILemmingStateChanger>? AllowedStates { get; init; }
    public SimpleSet<Orientation>? AllowedOrientations { get; init; }
    public FacingDirection? AllowedFacingDirection { get; init; }

    public GadgetStateAnimationController GetAnimationController()
    {
        var primaryAnimationBehaviour = PrimaryAnimation.GetAnimationBehaviour();
        var secondaryAnimationBehaviour = SecondaryAnimation?.GetAnimationBehaviour();

        return new GadgetStateAnimationController(
            primaryAnimationBehaviour,
            PrimaryAnimationStateTransitionIndex,
            secondaryAnimationBehaviour,
            SecondaryAnimationAction);
    }
}

public sealed class GadgetAnimationArchetypeData
{
    private GadgetStateAnimationBehaviour? _animationBehaviour;

    public required int SpriteWidth { get; init; }
    public required int SpriteHeight { get; init; }
    public required int Layer { get; init; }
    public required int InitialFrame { get; init; }
    public required int MinFrame { get; init; }
    public required int MaxFrame { get; init; }

    public GadgetStateAnimationBehaviour GetAnimationBehaviour()
    {
        return _animationBehaviour ??= new GadgetStateAnimationBehaviour(
            SpriteWidth,
            SpriteHeight,
            Layer,
            InitialFrame,
            MinFrame,
            MaxFrame);
    }

    public void Clear()
    {
        _animationBehaviour = null;
    }
}