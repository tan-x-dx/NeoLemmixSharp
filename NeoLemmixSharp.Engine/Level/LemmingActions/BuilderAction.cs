using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class BuilderAction : LemmingAction
{
    public static BuilderAction Instance { get; } = new();

    private BuilderAction()
    {
    }

    public override int Id => GameConstants.BuilderActionId;
    public override string LemmingActionName => "builder";
    public override int NumberOfAnimationFrames => GameConstants.BuilderAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => GameConstants.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.PhysicsFrame == 9)
        {
            LayBrick(lemming);

            return true;
        }

        if (lemming.PhysicsFrame == 10 &&
                 lemming.NumberOfBricksLeft <= 3)
        {
            // play sound/make visual cue
            return true;
        }

        if (lemming.PhysicsFrame != 0)
            return true;

        BuilderFrame0(lemming);
        lemming.ConstructivePositionFreeze = false;

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -2;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 4;

    private static void BuilderFrame0(Lemming lemming)
    {
        lemming.NumberOfBricksLeft--;

        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        if (TerrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, 2)))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (TerrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, 3)) ||
                 TerrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx + dx, 2)) ||
                 (TerrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx + dx, 10)) &&
                  lemming.NumberOfBricksLeft > 0))
        {
            lemmingPosition = orientation.Move(lemmingPosition, dx, 1);
            lemming.LevelPosition = lemmingPosition;
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (!lemming.ConstructivePositionFreeze)
        {
            lemmingPosition = orientation.Move(lemmingPosition, dx + dx, 1);
            lemming.LevelPosition = lemmingPosition;
        }

        if (TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 2)) ||
            TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 3)) ||
            TerrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, 3)) ||
            (TerrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx + dx, 10)) &&
             lemming.NumberOfBricksLeft > 0))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (lemming.NumberOfBricksLeft == 0)
        {
            ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.NumberOfBricksLeft = 12;
        lemming.ConstructivePositionFreeze = false;
    }

    public static void LayBrick(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dy = lemming.CurrentAction == Instance
            ? 1
            : 0;

        var brickPosition = lemming.LevelPosition;
        brickPosition = orientation.MoveUp(brickPosition, dy);
        TerrainManager.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        TerrainManager.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        TerrainManager.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        TerrainManager.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        TerrainManager.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        TerrainManager.SetSolidPixel(brickPosition, uint.MaxValue);
    }
}
