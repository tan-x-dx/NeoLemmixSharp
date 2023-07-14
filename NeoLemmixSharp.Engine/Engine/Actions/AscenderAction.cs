namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class AscenderAction : LemmingAction
{
    public const int NumberOfAscenderAnimationFrames = 1;

    public static AscenderAction Instance { get; } = new();

    private AscenderAction()
    {
    }

    public override int Id => 18;
    public override string LemmingActionName => "ascender";
    public override int NumberOfAnimationFrames => NumberOfAscenderAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        var levelPosition = lemming.LevelPosition;
        var orientation = lemming.Orientation;

        var dy = 0;
        while (dy < 2 &&
               lemming.AscenderProgress < 5 &&
               Terrain.PixelIsSolidToLemming(orientation.MoveUp(levelPosition, 1), lemming))
        {
            dy++;
            levelPosition = orientation.MoveUp(levelPosition, 1);
            lemming.LevelPosition = levelPosition;
            lemming.AscenderProgress++;
        }

        var pixel1IsSolid = Terrain.PixelIsSolidToLemming(orientation.MoveUp(levelPosition, 1), lemming);
        var pixel2IsSolid = Terrain.PixelIsSolidToLemming(orientation.MoveUp(levelPosition, 2), lemming);

        if (dy < 2 &&
            !pixel1IsSolid)
        {
            lemming.SetNextAction(WalkerAction.Instance);
        }
        else if ((lemming.AscenderProgress == 4 &&
                  pixel1IsSolid &&
                  pixel2IsSolid) ||
                 (lemming.AscenderProgress >= 5 &&
                  pixel1IsSolid))
        {
            var dx = lemming.FacingDirection.DeltaX;
            lemming.LevelPosition = orientation.MoveLeft(levelPosition, dx);
            FallerAction.Instance.TransitionLemmingToAction(lemming, true);
        }

        return true;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.AscenderProgress = 0;
    }
}