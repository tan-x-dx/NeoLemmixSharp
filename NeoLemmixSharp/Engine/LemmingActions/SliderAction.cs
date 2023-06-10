using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Util;
using System;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class SliderAction : LemmingAction
{
    public const int NumberOfSliderAnimationFrames = 1;

    public static SliderAction Instance { get; } = new();

    private SliderAction()
    {
    }

    public override int ActionId => 25;
    public override string LemmingActionName => "slider";
    public override int NumberOfAnimationFrames => NumberOfSliderAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
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
        var hasPixelAtLemmingPosition = SliderHasPixelAt(lemming, lemming.Orientation, lemming.LevelPosition, lemming.DehoistPin);

        if (hasPixelAtLemmingPosition &&
            !SliderHasPixelAt(lemming, lemming.Orientation, lemming.LevelPosition, lemming.Orientation.MoveDown(lemming.DehoistPin, 1)))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return false;
        }

        if (!SliderHasPixelAt(lemming, lemming.Orientation, lemming.Orientation.MoveUp(lemming.LevelPosition, Math.Min(maxYOffset, 7)), lemming.DehoistPin))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
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
                SwimmerAction.Instance.TransitionLemmingToAction(lemming, true);
                // ?? CueSoundEffect(SFX_SWIMMING, L.Position); ??
            }
            else
            {
                DrownerAction.Instance.TransitionLemmingToAction(lemming, true);
                // ?? CueSoundEffect(SFX_DROWNING, L.Position); ??
            }

            return true;
        }

        var leftPos = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
        if (!SliderHasPixelAt(lemming, lemming.Orientation, lemming.LevelPosition, leftPos))
            return true;

        lemming.LevelPosition = leftPos;
        WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
        return false;
    }

    private static bool SliderHasPixelAt(
        Lemming lemming,
        Orientation orientation,
        in LevelPosition levelPosition,
        in LevelPosition dehoistPin)
    {
        if (Terrain.GetPixelData(dehoistPin).IsSolidToLemming(lemming))
            return true;

        var result = false;
        if (orientation.MatchesHorizontally(levelPosition, dehoistPin) &&
            orientation.MatchesVertically(levelPosition, dehoistPin) &&
            true)
        {
            result = Terrain.GetPixelData(orientation.MoveDown(dehoistPin, 1)).IsSolidToLemming(lemming);
        }

        return result;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        lemming.DehoistPin = new LevelPosition(-1, -1);

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}