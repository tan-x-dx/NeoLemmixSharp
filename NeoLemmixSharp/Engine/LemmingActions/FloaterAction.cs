using System;

namespace NeoLemmixSharp.Engine.LemmingActions;

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

    public override int ActionId => 14;
    public override string LemmingActionName => "floater";
    public override int NumberOfAnimationFrames => NumberOfFloaterAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override bool CanBeAssignedPermanentSkill => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        var maxFallDistance = _floaterFallTable[lemming.AnimationFrame - 1];

        if (false)// if HasTriggerAt(L.LemX, L.LemY, trUpdraft)
        {
            maxFallDistance--;
        }

        var levelPosition = lemming.LevelPosition;
        var groundPixelDistance = Math.Max(FindGroundPixel(lemming.Orientation, levelPosition), 0);
        if (maxFallDistance > groundPixelDistance)
        {
            levelPosition = lemming.Orientation.MoveDown(levelPosition, groundPixelDistance);
            lemming.NextAction = WalkerAction.Instance;
        }
        else
        {
            levelPosition = lemming.Orientation.MoveDown(levelPosition, maxFallDistance);
        }

        lemming.LevelPosition = levelPosition;

        return true;
    }
}