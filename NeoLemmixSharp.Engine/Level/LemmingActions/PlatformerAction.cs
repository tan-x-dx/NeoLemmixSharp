using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class PlatformerAction : LemmingAction
{
    public static PlatformerAction Instance { get; } = new();

    private PlatformerAction()
    {
    }

    public override int Id => GameConstants.PlatformerActionId;
    public override string LemmingActionName => "platformer";
    public override int NumberOfAnimationFrames => GameConstants.PlatformerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => GameConstants.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        DoMainUpdate(lemming);

        if (lemming.AnimationFrame == 0)
        {
            lemming.ConstructivePositionFreeze = false;
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => animationFrame switch
    { 
        13 => -2,
        14 => -1,
        15 => -1,
        _ => -3
    };

    protected override int TopLeftBoundsDeltaY(int animationFrame) => 8;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => animationFrame switch
    {
        13 => 4,
        14 => 5,
        15 => 5,
        _ => 3
    };

    private static void DoMainUpdate(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var lemmingPosition = lemming.LevelPosition;

        if (lemming.AnimationFrame == 9)
        {
            lemming.PlacedBrick = LemmingCanPlatform(lemming, orientation);
            BuilderAction.LayBrick(lemming);

            return;
        }

        if (lemming.AnimationFrame == 10 && lemming.NumberOfBricksLeft <= 3)
        {
            // ?? CueSoundEffect(SFX_BUILDER_WARNING, L.Position) ??
            return;
        }

        if (lemming.AnimationFrame == 15)
        {
            if (!lemming.PlacedBrick)
            {
                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

                return;
            }

            if (PlatformerTerrainCheck(lemming, orientation.MoveRight(lemmingPosition, dx + dx)))
            {
                lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
                lemming.LevelPosition = lemmingPosition;

                WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

                return;
            }

            if (lemming.ConstructivePositionFreeze)
                return;

            lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
            lemming.LevelPosition = lemmingPosition;

            return;
        }

        if (lemming.AnimationFrame != 0)
            return;

        if (PlatformerTerrainCheck(lemming, orientation.MoveRight(lemmingPosition, dx + dx)) &&
            lemming.NumberOfBricksLeft > 1)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx);
            lemming.LevelPosition = lemmingPosition;
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (PlatformerTerrainCheck(lemming, orientation.MoveRight(lemmingPosition, dx + dx + dx)) &&
            lemming.NumberOfBricksLeft > 1)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx + dx);
            lemming.LevelPosition = lemmingPosition;
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (!lemming.ConstructivePositionFreeze)
        {
            lemmingPosition = orientation.MoveRight(lemmingPosition, dx + dx);
            lemming.LevelPosition = lemmingPosition;
        }

        lemming.NumberOfBricksLeft--; // Why are we doing this here, instead at the beginning of frame 15??

        if (lemming.NumberOfBricksLeft != 0)
            return;

        // stalling if there are pixels in the way:
        if (TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 1)))
        {
            lemmingPosition = orientation.MoveLeft(lemmingPosition, dx);
            lemming.LevelPosition = lemmingPosition;
        }

        ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    public static bool LemmingCanPlatform(
        Lemming lemming,
        Orientation orientation)
    {
        var lemmingPosition = lemming.LevelPosition;

        var result = !TerrainManager.PixelIsSolidToLemming(lemming, lemmingPosition) ||
                     !TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(lemmingPosition, 1)) ||
                     !TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(lemmingPosition, 2)) ||
                     !TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(lemmingPosition, 3)) ||
                     !TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(lemmingPosition, 4));

        var dx = lemming.FacingDirection.DeltaX;
        result = result && !TerrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, 1));
        result = result && !TerrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx + dx, 1));
        return result;
    }

    private static bool PlatformerTerrainCheck(
        Lemming lemming,
        LevelPosition pos)
    {
        return TerrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(pos, 1)) ||
               TerrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(pos, 2));
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.NumberOfBricksLeft = 12;
        lemming.ConstructivePositionFreeze = false;
    }
}