using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class BuilderAction : LemmingAction
{
    public static readonly BuilderAction Instance = new();

    private BuilderAction()
    {
    }

    public override int Id => LevelConstants.BuilderActionId;
    public override string LemmingActionName => "builder";
    public override int NumberOfAnimationFrames => LevelConstants.BuilderAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => LevelConstants.NonPermanentSkillPriority;

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
        var terrainManager = LevelScreen.TerrainManager;
        lemming.NumberOfBricksLeft--;

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        if (terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, 2)))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, 3)) ||
            terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx + dx, 2)) ||
            (terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx + dx, 10)) &&
             lemming.NumberOfBricksLeft > 0))
        {
            lemmingPosition = orientation.Move(lemmingPosition, dx, 1);
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (!lemming.ConstructivePositionFreeze)
        {
            lemmingPosition = orientation.Move(lemmingPosition, dx + dx, 1);
        }

        if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 2)) ||
            terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 3)) ||
            terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx, 3)) ||
            (terrainManager.PixelIsSolidToLemming(lemming, orientation.Move(lemmingPosition, dx + dx, 10)) &&
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
        var terrainManager = LevelScreen.TerrainManager;
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dy = lemming.CurrentAction == Instance
            ? 1
            : 0;

        var brickPosition = lemming.LevelPosition;
        brickPosition = orientation.MoveUp(brickPosition, dy);
        terrainManager.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        terrainManager.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        terrainManager.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        terrainManager.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        terrainManager.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        terrainManager.SetSolidPixel(brickPosition, uint.MaxValue);
    }
}