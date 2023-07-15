namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class BuilderAction : LemmingAction
{
    public const int NumberOfBuilderAnimationFrames = 16;

    public static BuilderAction Instance { get; } = new();

    private BuilderAction()
    {
    }

    public override int Id => 4;
    public override string LemmingActionName => "builder";
    public override int NumberOfAnimationFrames => NumberOfBuilderAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.AnimationFrame == 9)
        {
            LayBrick(lemming);
        }
        else if (lemming.AnimationFrame == 10 &&
                 lemming.NumberOfBricksLeft <= 3)
        {
            // play sound/make visual cue
        }
        else if (lemming.AnimationFrame == 0)
        {
            BuilderFrame0(lemming);
            lemming.ConstructivePositionFreeze = false;
        }

        return true;
    }

    private static void BuilderFrame0(Lemming lemming)
    {
        lemming.NumberOfBricksLeft--;

        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;
        var dx = lemming.FacingDirection.DeltaX;

        if (Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx, 2), lemming))
        {
            WalkerAction.Instance.TransitionLemmingToAction(lemming, true);

            return;
        }

        if (Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx, 3), lemming) ||
                 Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx + dx, 2), lemming) ||
                 (Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx + dx, 10), lemming) &&
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

        if (Terrain.PixelIsSolidToLemming(orientation.MoveUp(lemmingPosition, 2), lemming) ||
            Terrain.PixelIsSolidToLemming(orientation.MoveUp(lemmingPosition, 3), lemming) ||
            Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx, 3), lemming) ||
            (Terrain.PixelIsSolidToLemming(orientation.Move(lemmingPosition, dx + dx, 10), lemming) &&
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
}
