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

    public override int Id => GameConstants.ReacherActionId;
    public override string LemmingActionName => "reacher";
    public override int NumberOfAnimationFrames => GameConstants.ReacherAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => GameConstants.NonWalkerMovementPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        var emptyPixels = GetEmptyPixelCount(lemming, lemmingPosition);

        if (TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 5)) ||
            TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 6)) ||
            TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 7)) ||
            TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 8)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (lemming.AnimationFrame == 1 &&
            TerrainManager.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 9)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (emptyPixels <= _movementList[lemming.AnimationFrame])
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, emptyPixels + 1);
            lemming.LevelPosition = lemmingPosition;
            ShimmierAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        lemmingPosition = orientation.MoveUp(lemmingPosition, _movementList[lemming.AnimationFrame]);
        lemming.LevelPosition = lemmingPosition;
        if (lemming.AnimationFrame == 7)
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
        if (TerrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(lemmingPosition, 10)))
            return 0;

        if (TerrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(lemmingPosition, 11)))
            return 1;

        if (TerrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(lemmingPosition, 12)))
            return 2;

        if (TerrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(lemmingPosition, 13)))
            return 3;

        return 4;
    }
}