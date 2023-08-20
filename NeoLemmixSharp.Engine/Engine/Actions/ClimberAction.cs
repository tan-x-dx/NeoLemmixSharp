using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class ClimberAction : LemmingAction
{
    public static ClimberAction Instance { get; } = new();

    private ClimberAction()
    {
    }

    public override int Id => GameConstants.ClimberActionId;
    public override string LemmingActionName => "climber";
    public override int NumberOfAnimationFrames => GameConstants.ClimberAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => GameConstants.PermanentSkillPriority;

    // Be very careful when changing the terrain/hoister checks for climbers!
    // See http://www.lemmingsforums.net/index.php?topic=2506.0 first!
    public override bool UpdateLemming(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;
        var animationFrame = lemming.AnimationFrame;

        bool foundClip;

        if (animationFrame <= 3)
        {
            foundClip =
                Terrain.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, -dx, 6 + animationFrame)) ||
                (Terrain.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, -dx, 5 + animationFrame)) &&
                 !lemming.IsStartingAction);

            if (animationFrame == 0) // first triggered after 8 frames!
            {
                foundClip = foundClip && Terrain.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, -dx, 7));
            }

            if (foundClip)
            {
                // Don't fall below original position on hitting terrain in first cycle
                if (!lemming.IsStartingAction)
                {
                    lemmingPosition = orientation.MoveUp(lemmingPosition, 3 - animationFrame);
                    lemming.LevelPosition = lemmingPosition;
                }

                if (lemming.State.IsSlider)
                {
                    lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
                    lemming.LevelPosition = lemmingPosition;
                    SliderAction.Instance.TransitionLemmingToAction(lemming, false);

                    return true;
                }

                lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
                lemming.LevelPosition = lemmingPosition;
                FallerAction.Instance.TransitionLemmingToAction(lemming, true);
                lemming.DistanceFallen++; // Least-impact way to fix a fall distance inconsistency. See https://www.lemmingsforums.net/index.php?topic=5794.0

                return true;
            }

            if (Terrain.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 7 + animationFrame)))
                return true;

            // if-case prevents too deep bombing, see http://www.lemmingsforums.net/index.php?topic=2620.0
            if (!(lemming.IsStartingAction && animationFrame == 1))
            {
                lemmingPosition = orientation.MoveUp(lemmingPosition, animationFrame - 2);
                lemming.LevelPosition = lemmingPosition;
                lemming.IsStartingAction = false;
            }

            HoisterAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        lemmingPosition = orientation.MoveUp(lemmingPosition, 1);
        lemming.LevelPosition = lemmingPosition;
        lemming.IsStartingAction = false;

        foundClip = Terrain.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, -dx, 7));

        if (animationFrame == 7)
        {
            foundClip = foundClip && Terrain.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 7));
        }

        if (!foundClip)
            return true;

        lemmingPosition = orientation.MoveDown(lemmingPosition, 1);
        lemming.LevelPosition = lemmingPosition;

        if (lemming.State.IsSlider)
        {
            SliderAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
        lemming.LevelPosition = lemmingPosition;
        FallerAction.Instance.TransitionLemmingToAction(lemming, true);

        return true;
    }
}