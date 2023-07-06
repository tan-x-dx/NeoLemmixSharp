using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Gadgets;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class SliderAction : LemmingAction
{
    public const int NumberOfSliderAnimationFrames = 1;

    public static SliderAction Instance { get; } = new();

    private SliderAction()
    {
    }

    public override int Id => 25;
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
        var lemmingPosition = lemming.LevelPosition;
        var lemmingDehoistPosition = lemming.DehoistPin;

        var hasPixelAtLemmingPosition = SliderHasPixelAt(lemming, lemming.Orientation, lemmingPosition, lemmingDehoistPosition);

        if (hasPixelAtLemmingPosition &&
            !SliderHasPixelAt(lemming, lemming.Orientation, lemmingPosition, lemming.Orientation.MoveDown(lemmingDehoistPosition, 1)))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, false);
            return false;
        }

        if (!SliderHasPixelAt(lemming, lemming.Orientation, lemming.Orientation.MoveUp(lemmingPosition, Math.Min(maxYOffset, 7)), lemmingDehoistPosition))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            return false;
        }

        if (!hasPixelAtLemmingPosition)
            return true;

        var dx = lemming.FacingDirection.DeltaX;

        if (GadgetCollections.Waters.TryGetGadgetThatMatchesTypeAndOrientation(lemmingPosition, lemming.Orientation, out var water))
        {
            lemmingPosition = lemming.Orientation.MoveLeft(lemmingPosition, dx);
            lemming.LevelPosition = lemmingPosition;
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
            water!.OnLemmingInHitBox(lemming);

            return true;
        }

        var leftPos = lemming.Orientation.MoveLeft(lemmingPosition, dx);
        if (!SliderHasPixelAt(lemming, lemming.Orientation, lemmingPosition, leftPos))
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
        if (Terrain.PixelIsSolidToLemming(dehoistPin, lemming))
            return true;

        var result = false;
        if (orientation.MatchesHorizontally(levelPosition, dehoistPin) &&
            orientation.MatchesVertically(levelPosition, dehoistPin) &&
            true)
        {
            result = Terrain.PixelIsSolidToLemming(orientation.MoveDown(dehoistPin, 1), lemming);
        }

        return result;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        lemming.DehoistPin = new LevelPosition(-1, -1);

        base.TransitionLemmingToAction(lemming, turnAround);
    }
}