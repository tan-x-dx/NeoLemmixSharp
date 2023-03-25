using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering;
using static NeoLemmixSharp.Engine.LemmingActions.ILemmingAction;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class PlatformerAction : ILemmingAction
{
    public const int NumberOfPlatformerAnimationFrames = 16;

    public static PlatformerAction Instance { get; } = new();

    private PlatformerAction()
    {
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }
    public string LemmingActionName => "platformer";
    public int NumberOfAnimationFrames => NumberOfPlatformerAnimationFrames;

    public bool Equals(ILemmingAction? other) => other is PlatformerAction;
    public override bool Equals(object? obj) => obj is PlatformerAction;
    public override int GetHashCode() => nameof(PlatformerAction).GetHashCode();

    public void UpdateLemming(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;

        if (lemming.AnimationFrame == 9)
        {
            lemming.PlacedBrick = CommonMethods.LemmingCanPlatform(lemming);
            CommonMethods.LayBrick(lemming);
        }
        else if (lemming.AnimationFrame == 10 && lemming.NumberOfBricksLeft <= 3)
        {
            // ?? CueSoundEffect(SFX_BUILDER_WARNING, L.Position) ??
        }
        else if (lemming.AnimationFrame == 15)
        {
            if (!lemming.PlacedBrick)
            {
                CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, true);
            }
            else if (PlatformerTerrainCheck(lemming.Orientation.MoveRight(lemming.LevelPosition, dx + dx), lemming.Orientation))
            {
                lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, dx);
                CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, true);
            }
            else if (!lemming.ConstructivePositionFreeze)
            {
                lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, dx);
            }
        }
        else if (lemming.AnimationFrame == 0)
        {
            if (PlatformerTerrainCheck(lemming.Orientation.MoveRight(lemming.LevelPosition, dx + dx), lemming.Orientation) &&
                lemming.NumberOfBricksLeft > 1)
            {
                lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, dx);
                CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, true);
            }
            else if (PlatformerTerrainCheck(lemming.Orientation.MoveRight(lemming.LevelPosition, dx + dx + dx), lemming.Orientation) &&
                     lemming.NumberOfBricksLeft > 1)
            {
                lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, dx + dx);
                CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, true);
            }
            else
            {
                if (!lemming.ConstructivePositionFreeze)
                {
                    lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, dx + dx);
                }

                lemming.NumberOfBricksLeft--; // Why are we doing this here, instead at the beginning of frame 15??

                if (lemming.NumberOfBricksLeft == 0)
                {
                    // stalling if there are pixels in the way:
                    if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 1)).IsSolid)
                    {
                        lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
                    }

                    CommonMethods.TransitionToNewAction(lemming, ShruggerAction.Instance, false);
                }
            }
        }

        if (lemming.AnimationFrame == 0)
        {
            lemming.ConstructivePositionFreeze = false;
        }
    }

    private static bool PlatformerTerrainCheck(
        LevelPosition pos,
        IOrientation orientation)
    {
        return Terrain.GetPixelData(orientation.MoveUp(pos, 1)).IsSolid ||
               Terrain.GetPixelData(orientation.MoveUp(pos, 2)).IsSolid;
    }

    public void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
        lemming.NumberOfBricksLeft = LemmingConstants.StepsMax;
        lemming.ConstructivePositionFreeze = false;
    }
}