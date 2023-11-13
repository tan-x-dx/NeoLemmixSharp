using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class ReacherAction : LemmingAction
{
    public static ReacherAction Instance { get; } = new();

    private readonly int[] _movementList =
    {
        0, 3, 2, 2, 1, 1, 1, 0
    };

    private ReacherAction()
    {
    }

    public override int Id => LevelConstants.ReacherActionId;
    public override string LemmingActionName => "reacher";
    public override int NumberOfAnimationFrames => LevelConstants.ReacherAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => LevelConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var terrainManager = LevelConstants.TerrainManager;
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        var emptyPixels = GetEmptyPixelCount(lemming, lemmingPosition);

        if (terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 5)) ||
            terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 6)) ||
            terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 7)) ||
            terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 8)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (lemming.PhysicsFrame == 1 &&
            terrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 9)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (emptyPixels <= _movementList[lemming.PhysicsFrame])
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, emptyPixels + 1);
            lemming.LevelPosition = lemmingPosition;
            ShimmierAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        lemmingPosition = orientation.MoveUp(lemmingPosition, _movementList[lemming.PhysicsFrame]);
        lemming.LevelPosition = lemmingPosition;
        if (lemming.PhysicsFrame == 7)
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 9;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

    private static int GetEmptyPixelCount(
        Lemming lemming,
        LevelPosition lemmingPosition)
    {
        var terrainManager = LevelConstants.TerrainManager;
        if (terrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(lemmingPosition, 10)))
            return 0;

        if (terrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(lemmingPosition, 11)))
            return 1;

        if (terrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(lemmingPosition, 12)))
            return 2;

        if (terrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(lemmingPosition, 13)))
            return 3;

        return 4;
    }
}