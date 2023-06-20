namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class AscenderAction : LemmingAction
{
    public const int NumberOfAscenderAnimationFrames = 1;

    public static AscenderAction Instance { get; } = new();

    private AscenderAction()
    {
    }

    public override int ActionId => 1;
    public override string LemmingActionName => "ascender";
    public override int NumberOfAnimationFrames => NumberOfAscenderAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var levelPosition = lemming.LevelPosition;

        var dy = 0;
        while (dy < 2 &&
               lemming.AscenderProgress < 5 &&
               Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveUp(levelPosition, 1), lemming))
        {
            dy++;
            lemming.LevelPosition = levelPosition = lemming.Orientation.MoveUp(levelPosition, 1);
            lemming.AscenderProgress++;
        }

        var pixel1IsSolid = Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveUp(levelPosition, 1), lemming);
        var pixel2IsSolid = Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveUp(levelPosition, 2), lemming);

        if (dy < 2 &&
            !pixel1IsSolid)
        {
            lemming.NextAction = WalkerAction.Instance;
        }
        else if ((lemming.AscenderProgress == 4 &&
                  pixel1IsSolid &&
                  pixel2IsSolid) ||
                 (lemming.AscenderProgress >= 5 &&
                  pixel1IsSolid))
        {
            var dx = lemming.FacingDirection.DeltaX;
            lemming.LevelPosition = lemming.Orientation.MoveLeft(levelPosition, dx);
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