namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class ReacherAction : LemmingAction
{
    public const int NumberOfReacherAnimationFrames = 8;

    public static ReacherAction Instance { get; } = new();

    private readonly int[] _movementList =
    {
        0, 3, 2, 2, 1, 1, 1, 0
    };

    private ReacherAction()
    {
    }

    public override int ActionId => 22;
    public override string LemmingActionName => "reacher";
    public override int NumberOfAnimationFrames => NumberOfReacherAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var lemmingPosition = lemming.LevelPosition;
        int emptyPixels;
        if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemmingPosition, 10)).IsSolid)
        {
            emptyPixels = 0;
        }
        else if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemmingPosition, 11)).IsSolid)
        {
            emptyPixels = 1;
        }
        else if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemmingPosition, 12)).IsSolid)
        {
            emptyPixels = 2;
        }
        else if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemmingPosition, 13)).IsSolid)
        {
            emptyPixels = 3;
        }
        else
        {
            emptyPixels = 4;
        }

        if (Terrain.GetPixelData(lemming.Orientation.MoveUp(lemmingPosition, 5)).IsSolid ||
            Terrain.GetPixelData(lemming.Orientation.MoveUp(lemmingPosition, 6)).IsSolid ||
            Terrain.GetPixelData(lemming.Orientation.MoveUp(lemmingPosition, 7)).IsSolid ||
            Terrain.GetPixelData(lemming.Orientation.MoveUp(lemmingPosition, 8)).IsSolid)
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
        else if (lemming.AnimationFrame == 1 &&
                 Terrain.GetPixelData(lemming.Orientation.MoveUp(lemmingPosition, 9)).IsSolid)
        {
            FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        }
        else if (emptyPixels <= _movementList[lemming.AnimationFrame])
        {
            lemmingPosition = lemming.Orientation.MoveUp(lemmingPosition, emptyPixels + 1);
            lemming.LevelPosition = lemmingPosition;
            ShimmierAction.Instance.TransitionLemmingToAction(lemming, false);
        }
        else
        {
            lemmingPosition = lemming.Orientation.MoveUp(lemmingPosition, _movementList[lemming.AnimationFrame]);
            lemming.LevelPosition = lemmingPosition;
            if (lemming.AnimationFrame == 7)
            {
                FallerAction.Instance.TransitionLemmingToAction(lemming, false);
            }
        }

        return true;
    }
}