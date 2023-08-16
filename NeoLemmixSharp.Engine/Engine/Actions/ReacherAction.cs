using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Engine.Actions;

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
    public override int CursorSelectionPriorityValue => 0;

    public override bool UpdateLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        var emptyPixels = GetEmptyPixelCount(lemming, lemmingPosition);

        if (Terrain.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 5)) ||
            Terrain.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 6)) ||
            Terrain.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 7)) ||
            Terrain.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 8)))
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);

            return true;
        }

        if (lemming.AnimationFrame == 1 &&
            Terrain.PixelIsSolidToLemming(lemming, orientation.MoveUp(lemmingPosition, 9)))
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

    private static int GetEmptyPixelCount(
        Lemming lemming,
        LevelPosition lemmingPosition)
    {
        if (Terrain.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(lemmingPosition, 10)))
            return 0;

        if (Terrain.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(lemmingPosition, 11)))
            return 1;

        if (Terrain.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(lemmingPosition, 12)))
            return 2;

        if (Terrain.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(lemmingPosition, 13)))
            return 3;

        return 4;
    }
}