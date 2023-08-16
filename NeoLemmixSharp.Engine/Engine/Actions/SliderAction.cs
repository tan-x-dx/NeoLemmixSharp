using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Gadgets.Collections;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class SliderAction : LemmingAction
{
    public static SliderAction Instance { get; } = new();

    private SliderAction()
    {
    }

    public override int Id => GameConstants.SliderActionId;
    public override string LemmingActionName => "slider";
    public override int NumberOfAnimationFrames => GameConstants.SliderAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => 1;

    public override bool UpdateLemming(Lemming lemming)
    {
        // if ((L.LemX <= 0) and(L.LemDX = -1)) or((L.LemX >= Level.Info.Width - 1) and(L.LemDX = 1)) then
        //      RemoveLemming(L, RM_NEUTRAL); // shouldn't get to this point but just in case

        var orientation = lemming.Orientation;
        lemming.LevelPosition = orientation.MoveDown(lemming.LevelPosition, 1);
        if (!SliderTerrainChecks(lemming, orientation) &&
            lemming.CurrentAction == DrownerAction.Instance)
            return false;

        lemming.LevelPosition = orientation.MoveDown(lemming.LevelPosition, 1);
        if (SliderTerrainChecks(lemming, orientation))
            return true;

        return lemming.CurrentAction != DrownerAction.Instance;
    }

    public static bool SliderTerrainChecks(
        Lemming lemming,
        Orientation orientation,
        int maxYOffset = 7)
    {
        var lemmingPosition = lemming.LevelPosition;
        var lemmingDehoistPosition = lemming.DehoistPin;

        var hasPixelAtLemmingPosition = SliderHasPixelAt(lemming, orientation, lemmingPosition, lemmingDehoistPosition);

        if (hasPixelAtLemmingPosition &&
            !SliderHasPixelAt(lemming, orientation, lemmingPosition, orientation.MoveDown(lemmingDehoistPosition, 1)))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return false;
        }

        if (!SliderHasPixelAt(lemming, orientation, orientation.MoveUp(lemmingPosition, Math.Min(maxYOffset, 7)), lemmingDehoistPosition))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return false;
        }

        if (!hasPixelAtLemmingPosition)
            return true;

        var dx = lemming.FacingDirection.DeltaX;

        if (GadgetCollections.Waters.TryGetGadgetThatMatchesTypeAndOrientation(lemming, lemmingPosition, out var water))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            lemming.LevelPosition = lemmingPosition;
            if (lemming.State.IsSwimmer)
            {
                SwimmerAction.Instance.TransitionLemmingToAction(lemming, true);
                // ?? CueSoundEffect(SFX_SWIMMING, L.Position); ??
            }
            else
            {
                DrownerAction.Instance.TransitionLemmingToAction(lemming, true);
                // ?? CueSoundEffect(SFX_DROWNING, L.Position); ??
            }
            //water.OnLemmingInHitBox(lemming);

            return true;
        }

        var leftPos = orientation.MoveLeft(lemmingPosition, dx);
        if (!SliderHasPixelAt(lemming, orientation, lemmingPosition, leftPos))
            return true;

        lemming.LevelPosition = leftPos;
        WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
        return false;
    }

    private static bool SliderHasPixelAt(
        Lemming lemming,
        Orientation orientation,
        LevelPosition levelPosition,
        LevelPosition dehoistPin)
    {
        if (Terrain.PixelIsSolidToLemming(lemming, dehoistPin))
            return true;

        var result = false;
        if (orientation.MatchesHorizontally(levelPosition, dehoistPin) &&
            orientation.MatchesVertically(levelPosition, dehoistPin) &&
            true)
        {
            result = Terrain.PixelIsSolidToLemming(lemming, orientation.MoveDown(dehoistPin, 1));
        }

        return result;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        lemming.DehoistPin = new LevelPosition(-1, -1);

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}