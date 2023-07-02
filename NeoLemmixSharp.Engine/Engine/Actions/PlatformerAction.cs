using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class PlatformerAction : LemmingAction
{
    public const int NumberOfPlatformerAnimationFrames = 16;

    public static PlatformerAction Instance { get; } = new();

    private PlatformerAction()
    {
    }

    public override int Id => 21;
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
            else if (PlatformerTerrainCheck(lemming, lemming.Orientation.MoveRight(lemmingPosition, dx + dx), lemming.Orientation))
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
            if (PlatformerTerrainCheck(lemming, lemming.Orientation.MoveRight(lemmingPosition, dx + dx), lemming.Orientation) &&
                lemming.NumberOfBricksLeft > 1)
            {
                lemmingPosition = lemming.Orientation.MoveRight(lemmingPosition, dx);
                lemming.LevelPosition = lemmingPosition;
                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
            }
            else if (PlatformerTerrainCheck(lemming, lemming.Orientation.MoveRight(lemmingPosition, dx + dx + dx), lemming.Orientation) &&
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
                    if (Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveUp(lemmingPosition, 1), lemming))
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

        var result = !Terrain.PixelIsSolidToLemming(lemmingPosition, lemming) ||
                     !Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveRight(lemmingPosition, 1), lemming) ||
                     !Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveRight(lemmingPosition, 2), lemming) ||
                     !Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveRight(lemmingPosition, 3), lemming) ||
                     !Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveRight(lemmingPosition, 4), lemming);

        var dx = lemming.FacingDirection.DeltaX;
        result = result && !Terrain.PixelIsSolidToLemming(lemming.Orientation.Move(lemmingPosition, dx, 1), lemming);
        result = result && !Terrain.PixelIsSolidToLemming(lemming.Orientation.Move(lemmingPosition, dx + dx, 1), lemming);
        return result;
    }

    private static bool PlatformerTerrainCheck(
        Lemming lemming,
        LevelPosition pos,
        Orientation orientation)
    {
        return Terrain.PixelIsSolidToLemming(orientation.MoveUp(pos, 1), lemming) ||
               Terrain.PixelIsSolidToLemming(orientation.MoveUp(pos, 2), lemming);
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.NumberOfBricksLeft = 12;
        lemming.ConstructivePositionFreeze = false;
    }
}