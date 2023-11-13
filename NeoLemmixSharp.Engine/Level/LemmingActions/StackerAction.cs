using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class StackerAction : LemmingAction
{
    public static StackerAction Instance { get; } = new();

    private StackerAction()
    {
    }

    public override int Id => LevelConstants.StackerActionId;
    public override string LemmingActionName => "stacker";
    public override int NumberOfAnimationFrames => LevelConstants.StackerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => LevelConstants.NonPermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.PhysicsFrame == 7)
        {
            lemming.PlacedBrick = LayStackBrick(lemming);
        }
        else if (lemming.PhysicsFrame == 0)
        {
            lemming.NumberOfBricksLeft--;

            if (lemming.NumberOfBricksLeft < 3)
            {
                // ?? CueSoundEffect(SFX_BUILDER_WARNING, L.Position); ??
            }

            if (!lemming.PlacedBrick)
            {
                // Relax the check on the first brick
                // for details see http://www.lemmingsforums.net/index.php?topic=2862.0
                if (lemming.NumberOfBricksLeft < 7 ||
                    !MayPlaceNextBrick(lemming))
                {
                    WalkerAction.Instance.TransitionLemmingToAction(lemming, true);
                }
            }
            else if (lemming.NumberOfBricksLeft == 0)
            {
                ShruggerAction.Instance.TransitionLemmingToAction(lemming, false);
            }
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -2;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 10;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    private static bool MayPlaceNextBrick(Lemming lemming)
    {
        var terrainManager = LevelConstants.TerrainManager;
        var orientation = lemming.Orientation;
        var brickPosition = lemming.LevelPosition;
        brickPosition = orientation.MoveUp(brickPosition, 9 - lemming.NumberOfBricksLeft);

        var dx = lemming.FacingDirection.DeltaX;

        return !(terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(brickPosition, dx)) &&
                 terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(brickPosition, dx + dx)) &&
                 terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveRight(brickPosition, dx + dx + dx)));
    }

    private static bool LayStackBrick(Lemming lemming)
    {
        var terrainManager = LevelConstants.TerrainManager;
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dy = lemming.StackLow ? -1 : 0;
        var brickPosition = orientation.Move(lemming.LevelPosition, dx, 9 + dy - lemming.NumberOfBricksLeft);

        var result = false;

        for (var i = 0; i < 3; i++)
        {
            if (!terrainManager.PixelIsSolidToLemming(lemming, brickPosition))
            {
                terrainManager.SetSolidPixel(brickPosition, uint.MaxValue);
                result = true;
            }

            brickPosition = orientation.MoveRight(brickPosition, dx);
        }

        return result;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.NumberOfBricksLeft = 8;
    }
}