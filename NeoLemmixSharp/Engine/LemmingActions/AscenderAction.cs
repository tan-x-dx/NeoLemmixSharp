namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class AscenderAction : LemmingAction
{
    public const int NumberOfAscenderAnimationFrames = 1;

    public static AscenderAction Instance { get; } = new();

    private AscenderAction()
    {
    }

    protected override int ActionId => 1;
    public override string LemmingActionName => "ascender";
    public override int NumberOfAnimationFrames => NumberOfAscenderAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        var dy = 0;
        while (dy < 2 &&
               lemming.AscenderProgress < 5 &&
               Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 1)).IsSolid)
        {
            dy++;
            lemming.LevelPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 1);
            lemming.AscenderProgress++;
        }

        var pixel1 = Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 1));
        var pixel2 = Terrain.GetPixelData(lemming.Orientation.MoveUp(lemming.LevelPosition, 2));

        if (dy < 2 &&
            !pixel1.IsSolid)
        {
            lemming.NextAction = WalkerAction.Instance;
        }
        else if ((lemming.AscenderProgress == 4 &&
                  pixel1.IsSolid &&
                  pixel2.IsSolid) ||
                 (lemming.AscenderProgress >= 5 &&
                  pixel1.IsSolid))
        {
            var dx = lemming.FacingDirection.DeltaX;
            lemming.LevelPosition = lemming.Orientation.MoveLeft(lemming.LevelPosition, dx);
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