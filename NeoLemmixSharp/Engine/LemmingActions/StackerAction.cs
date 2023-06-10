namespace NeoLemmixSharp.Engine.LemmingActions;

public sealed class StackerAction : LemmingAction
{
    public const int NumberOfStackerAnimationFrames = 8;

    public static StackerAction Instance { get; } = new();

    private StackerAction()
    {
    }

    public override int ActionId => 27;
    public override string LemmingActionName => "stacker";
    public override int NumberOfAnimationFrames => NumberOfStackerAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.AnimationFrame == 7)
        {
            lemming.PlacedBrick = LayStackBrick(lemming);
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

    private static bool MayPlaceNextBrick(Lemming lemming)
    {
        var brickPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 9 - lemming.NumberOfBricksLeft);
        var dx = lemming.FacingDirection.DeltaX;

        return !(Terrain.GetPixelData(lemming.Orientation.MoveRight(brickPosition, dx)).IsSolidToLemming(lemming) &&
                 Terrain.GetPixelData(lemming.Orientation.MoveRight(brickPosition, dx + dx)).IsSolidToLemming(lemming) &&
                 Terrain.GetPixelData(lemming.Orientation.MoveRight(brickPosition, dx + dx + dx)).IsSolidToLemming(lemming));
    }

    private static bool LayStackBrick(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        var dy = lemming.StackLow ? -1 : 0;
        var brickPosition = lemming.Orientation.Move(lemming.LevelPosition, dx, 9 + dy - lemming.NumberOfBricksLeft);

        var result = false;

        for (var i = 0; i < 3; i++)
        {
            if (!Terrain.GetPixelData(brickPosition).IsSolidToLemming(lemming))
            {
                Terrain.SetSolidPixel(brickPosition, uint.MaxValue);
                result = true;
            }

            brickPosition = lemming.Orientation.MoveRight(brickPosition, dx);
        }

        return result;
    }

    public override void TransitionLemmingToAction(Lemming lemming, bool turnAround)
    {
        base.TransitionLemmingToAction(lemming, turnAround);

        lemming.NumberOfBricksLeft = 8;
    }
}