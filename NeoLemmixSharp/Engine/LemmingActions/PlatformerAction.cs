using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class PlatformerAction : LemmingAction
{
    public const int NumberOfPlatformerAnimationFrames = 16;

    public static PlatformerAction Instance { get; } = new();

    private PlatformerAction()
    {
    }

    public override int ActionId => 21;
    public override string LemmingActionName => "platformer";
    public override int NumberOfAnimationFrames => NumberOfPlatformerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        var lemmingPosition = lemming.LevelPosition;

        if (lemming.AnimationFrame == 9)
        {
            lemming.PlacedBrick = LemmingCanPlatform(lemming);
            LayBrick(lemming);
        }
        else if (lemming.AnimationFrame == 10 && lemming.NumberOfBricksLeft <= 3)
        {
            // ?? CueSoundEffect(SFX_BUILDER_WARNING, L.Position) ??
        }
        else if (lemming.AnimationFrame == 15)
        {
            if (!lemming.PlacedBrick)
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
            }
            else if (PlatformerTerrainCheck(lemming.Orientation.MoveRight(lemmingPosition, dx + dx), lemming.Orientation))
            {
                lemmingPosition = lemming.Orientation.MoveRight(lemmingPosition, dx);
                lemming.LevelPosition = lemmingPosition;

                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
            }
            else if (!lemming.ConstructivePositionFreeze)
            {
                lemmingPosition = lemming.Orientation.MoveRight(lemmingPosition, dx);
                lemming.LevelPosition = lemmingPosition;
            }
        }
        else if (lemming.AnimationFrame == 0)
        {
            if (PlatformerTerrainCheck(lemming.Orientation.MoveRight(lemmingPosition, dx + dx), lemming.Orientation) &&
                lemming.NumberOfBricksLeft > 1)
            {
                lemmingPosition = lemming.Orientation.MoveRight(lemmingPosition, dx);
                lemming.LevelPosition = lemmingPosition;
                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
            }
            else if (PlatformerTerrainCheck(lemming.Orientation.MoveRight(lemmingPosition, dx + dx + dx), lemming.Orientation) &&
                     lemming.NumberOfBricksLeft > 1)
            {
                lemmingPosition = lemming.Orientation.MoveRight(lemmingPosition, dx + dx);
                lemming.LevelPosition = lemmingPosition;
                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
            }
            else
            {
                if (!lemming.ConstructivePositionFreeze)
                {
                    lemmingPosition = lemming.Orientation.MoveRight(lemmingPosition, dx + dx);
                    lemming.LevelPosition = lemmingPosition;
                }

                lemming.NumberOfBricksLeft--; // Why are we doing this here, instead at the beginning of frame 15??

                if (lemming.NumberOfBricksLeft == 0)
                {
                    // stalling if there are pixels in the way:
                    if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemmingPosition, 1)).IsSolid)
                    {
                        lemmingPosition = lemming.Orientation.MoveLeft(lemmingPosition, dx);
                        lemming.LevelPosition = lemmingPosition;
                    }

                    ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);
                }
            }
        }

        if (lemming.AnimationFrame == 0)
        {
            lemming.ConstructivePositionFreeze = false;
        }

        return true;
    }

    public static bool LemmingCanPlatform(Lemming lemming)
    {
        var lemmingPosition = lemming.LevelPosition;

        var result = !Terrain.GetPixelData(lemmingPosition).IsSolid ||
                     !Terrain.GetPixelData(lemming.Orientation.MoveRight(lemmingPosition, 1)).IsSolid ||
                     !Terrain.GetPixelData(lemming.Orientation.MoveRight(lemmingPosition, 2)).IsSolid ||
                     !Terrain.GetPixelData(lemming.Orientation.MoveRight(lemmingPosition, 3)).IsSolid ||
                     !Terrain.GetPixelData(lemming.Orientation.MoveRight(lemmingPosition, 4)).IsSolid;

        var dx = lemming.FacingDirection.DeltaX;
        result = result && !Terrain.GetPixelData(lemming.Orientation.Move(lemmingPosition, dx, 1)).IsSolid;
        result = result && !Terrain.GetPixelData(lemming.Orientation.Move(lemmingPosition, dx + dx, 1)).IsSolid;
        return result;
    }

    private static bool PlatformerTerrainCheck(
        in LevelPosition pos,
        Orientation orientation)
    {
        return Terrain.GetPixelData(orientation.MoveUp(pos, 1)).IsSolid ||
               Terrain.GetPixelData(orientation.MoveUp(pos, 2)).IsSolid;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.NumberOfBricksLeft = 12;
        lemming.ConstructivePositionFreeze = false;
    }
}