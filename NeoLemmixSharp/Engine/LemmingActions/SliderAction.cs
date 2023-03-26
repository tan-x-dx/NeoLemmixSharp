using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering;
using System;
using static NeoLemmixSharp.Engine.LemmingActions.ILemmingAction;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class SliderAction : ILemmingAction
{
    public const int NumberOfSliderAnimationFrames = 1;

    public static SliderAction Instance { get; } = new();

    private SliderAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "slider";
    public int NumberOfAnimationFrames => NumberOfSliderAnimationFrames;
    public bool IsOneTimeAction => false;

    public bool Equals(ILemmingAction? other) => other is SliderAction;
    public override bool Equals(object? obj) => obj is SliderAction;
    public override int GetHashCode() => nameof(SliderAction).GetHashCode();

    public bool UpdateLemming(Lemming lemming)
    {
        // if ((L.LemX <= 0) and(L.LemDX = -1)) or((L.LemX >= Level.Info.Width - 1) and(L.LemDX = 1)) then
        //      RemoveLemming(L, RM_NEUTRAL); // shouldn't get to this point but just in case

        lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 1);
        if (!SliderTerrainChecks(lemming) &&
            lemming.CurrentAction == DrownerAction.Instance)
            return false;

        lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 1);
        if (SliderTerrainChecks(lemming))
            return true;

        return lemming.CurrentAction != DrownerAction.Instance;
    }

    private static bool SliderTerrainChecks(Lemming lemming, int maxYOffset = 7)
    {
        var hasPixelAtLemmingPosition = SliderHasPixelAt(lemming.Orientation, lemming.LevelPosition, lemming.DehoistPin);

        if (hasPixelAtLemmingPosition &&
            !SliderHasPixelAt(lemming.Orientation, lemming.LevelPosition, lemming.Orientation.MoveDown(lemming.DehoistPin, 1)))
        {
            CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, false);
            return false;
        }

        if (!SliderHasPixelAt(lemming.Orientation, lemming.Orientation.MoveUp(lemming.LevelPosition, Math.Min(maxYOffset, 7)), lemming.DehoistPin))
        {
            CommonMethods.TransitionToNewAction(lemming, FallerAction.Instance, false);
            return false;
        }

        if (!hasPixelAtLemmingPosition)
            return true;

        var dx = lemming.FacingDirection.DeltaX;

        if (false) // HasTriggerAt(L.LemX - L.LemDX, L.LemY, trWater, L)
        {
            lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
            if (lemming.IsSwimmer)
            {
                CommonMethods.TransitionToNewAction(lemming, SwimmerAction.Instance, true);
                // ?? CueSoundEffect(SFX_SWIMMING, L.Position); ??
            }
            else
            {
                CommonMethods.TransitionToNewAction(lemming, DrownerAction.Instance, true);
                // ?? CueSoundEffect(SFX_DROWNING, L.Position); ??
            }

            return true;
        }

        var leftPos = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
        if (!SliderHasPixelAt(lemming.Orientation, lemming.LevelPosition, leftPos))
            return true;

        lemming.LevelPosition = leftPos;
        CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, true);
        return false;
    }
    private static bool SliderHasPixelAt(
        IOrientation orientation,
        LevelPosition levelPosition,
        LevelPosition dehoistPin)
    {
        if (Terrain.GetPixelData(dehoistPin).IsSolid)
            return true;

        var result = false;
        if (orientation.MatchesHorizontally(levelPosition, dehoistPin) &&
            orientation.MatchesVertically(levelPosition, dehoistPin) &&
            true)
        {
            result = Terrain.GetPixelData(orientation.MoveDown(dehoistPin, 1)).IsSolid;
        }

        return result;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}