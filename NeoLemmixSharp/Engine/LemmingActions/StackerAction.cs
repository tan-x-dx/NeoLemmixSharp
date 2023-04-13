namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class StackerAction : LemmingAction
{
    public const int NumberOfStackerAnimationFrames = 8;

    public static StackerAction Instance { get; } = new();

    private StackerAction()
    {
    }

    protected override int ActionId => 27;
    public override string LemmingActionName => "stacker";
    public override int NumberOfAnimationFrames => NumberOfStackerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.AnimationFrame == 7)
        {
            lemming.PlacedBrick = CommonMethods.LayStackBrick(lemming);
        }
        else if (lemming.AnimationFrame == 0)
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
                    CommonMethods.TransitionToNewAction(lemming, WalkerAction.Instance, true);
                }
            }
            else if (lemming.NumberOfBricksLeft == 0)
            {
                CommonMethods.TransitionToNewAction(lemming, ShruggerAction.Instance, false);
            }
        }

        return true;
    }

    private static bool MayPlaceNextBrick(Lemming lemming)
    {
        var brickPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 9 - lemming.NumberOfBricksLeft);
        var dx = lemming.FacingDirection.DeltaX;

        return !(Terrain.GetPixelData(lemming.Orientation.MoveRight(brickPosition, dx)).IsSolid &&
                 Terrain.GetPixelData(lemming.Orientation.MoveRight(brickPosition, dx + dx)).IsSolid &&
                 Terrain.GetPixelData(lemming.Orientation.MoveRight(brickPosition, dx + dx + dx)).IsSolid);
    }

    public override void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction)
    {
        lemming.NumberOfBricksLeft = 8;
    }
}