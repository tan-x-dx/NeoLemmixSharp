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

    protected override int ActionId => 21;
    public override string LemmingActionName => "platformer";
    public override int NumberOfAnimationFrames => NumberOfPlatformerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;

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
            else if (PlatformerTerrainCheck(lemming.Orientation.MoveRight(lemming.LevelPosition, dx + dx), lemming.Orientation))
            {
                lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, dx);
                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
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
                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
            }
            else if (PlatformerTerrainCheck(lemming.Orientation.MoveRight(lemming.LevelPosition, dx + dx + dx), lemming.Orientation) &&
                     lemming.NumberOfBricksLeft > 1)
            {
                lemming.LevelPosition = lemming.Orientation.MoveRight(lemming.LevelPosition, dx + dx);
                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
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
        var result = !Terrain.GetPixelData(lemming.LevelPosition).IsSolid ||
                     !Terrain.GetPixelData(lemming.Orientation.MoveRight(lemming.LevelPosition, 1)).IsSolid ||
                     !Terrain.GetPixelData(lemming.Orientation.MoveRight(lemming.LevelPosition, 2)).IsSolid ||
                     !Terrain.GetPixelData(lemming.Orientation.MoveRight(lemming.LevelPosition, 3)).IsSolid ||
                     !Terrain.GetPixelData(lemming.Orientation.MoveRight(lemming.LevelPosition, 4)).IsSolid;

        var dx = lemming.FacingDirection.DeltaX;
        result = result && !Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx, 1)).IsSolid;
        result = result && !Terrain.GetPixelData(lemming.Orientation.Move(lemming.LevelPosition, dx + dx, 1)).IsSolid;
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