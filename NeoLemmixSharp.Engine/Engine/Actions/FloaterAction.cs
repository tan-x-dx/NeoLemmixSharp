using NeoLemmixSharp.Engine.Engine.Gadgets;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class FloaterAction : LemmingAction
{
    public const int NumberOfFloaterAnimationFrames = 17;

    public static FloaterAction Instance { get; } = new();

    private readonly int[] _floaterFallTable =
    {
        3, 3, 3, 3, -1, 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2
    };

    private FloaterAction()
    {
    }

    public override int Id => 2;
    public override string LemmingActionName => "floater";
    public override int NumberOfAnimationFrames => NumberOfFloaterAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        var maxFallDistance = _floaterFallTable[lemming.AnimationFrame - 1];

        var orientation = lemming.Orientation;
        var levelPosition = lemming.LevelPosition;

        if (GadgetCollections.Updrafts.TryGetGadgetThatMatchesTypeAndOrientation(levelPosition, orientation.GetOpposite(), out _))
        {
            maxFallDistance--;
        }

        var groundPixelDistance = Math.Max(FindGroundPixel(lemming, orientation, levelPosition), 0);
        if (maxFallDistance > groundPixelDistance)
        {
            levelPosition = orientation.MoveDown(levelPosition, groundPixelDistance);
            lemming.SetNextAction(WalkerAction.Instance);
        }
        else
        {
            levelPosition = orientation.MoveDown(levelPosition, maxFallDistance);
        }

        lemming.LevelPosition = levelPosition;

        return true;
    }
}