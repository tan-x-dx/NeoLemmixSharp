using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.LemmingActions.ILemmingAction;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class WalkerAction : ILemmingAction
{
    public const int NumberOfWalkerAnimationFrames = 8;

    public static WalkerAction Instance { get; } = new();

    private WalkerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "walker";
    public int NumberOfAnimationFrames => NumberOfWalkerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is WalkerAction;
    public override bool Equals(object? obj) => obj is WalkerAction;
    public override int GetHashCode() => nameof(WalkerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, dx);
        var dy = FindGroundPixel(lemming.Orientation, lemming.LevelPosition);

        if (dy > 0 &&
            lemming.IsSlider &&
            CommonMethods.LemmingCanDehoist(lemming, true))
        {
            lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
            CommonMethods.TransitionToNewAction(lemming, DehoisterAction.Instance, true);
            return;
        }

        if (dy < -6)
        {
            if (lemming.IsClimber)
            {
                CommonMethods.TransitionToNewAction(lemming, ClimberAction.Instance, false);
            }
            else
            {
                lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
                lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
            }
        }
        else if (dy < -2)
        {
            CommonMethods.TransitionToNewAction(lemming, AscenderAction.Instance, false);
            lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 2);
        }
        else if (dy < 1)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, dy);
        }

        // Get new ground pixel again in case the Lem has turned
        dy = FindGroundPixel(lemming.Orientation, lemming.LevelPosition);

        if (dy > 3)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, 4);
            CommonMethods.TransitionToNewAction(lemming, FallerAction.Instance, false);
        }
        else if (dy > 0)
        {
            lemming.LevelPosition = lemming.Orientation.MoveDown(lemming.LevelPosition, dy);
        }
    }

    private static int FindGroundPixel(
        IOrientation orientation,
        LevelPosition levelPosition)
    {
        // Find the new ground pixel
        // If Result = 4, then at least 4 pixels are air below (X, Y)
        // If Result = -7, then at least 7 pixels are terrain above (X, Y)
        var result = 0;
        if (Terrain.GetPixelData(levelPosition).IsSolid)
        {
            while (Terrain.GetPixelData(orientation.MoveUp(levelPosition, 1 - result)).IsSolid &&
                   result > -7)
            {
                result--;
            }

            return result;
        }

        result = 1;
        while (!Terrain.GetPixelData(orientation.MoveDown(levelPosition, result)).IsSolid &&
               result < 4)
        {
            result++;
        }

        return result;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
    }
}