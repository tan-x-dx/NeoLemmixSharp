using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class FloaterAction : LemmingAction
{
    public static FloaterAction Instance { get; } = new();

    private readonly int[] _floaterFallTable =
    {
        3, 3, 3, 3, -1, 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2
    };

    private FloaterAction()
    {
    }

    public override int Id => GameConstants.FloaterActionId;
    public override string LemmingActionName => "floater";
    public override int NumberOfAnimationFrames => GameConstants.FloaterAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => GameConstants.PermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var maxFallDistance = _floaterFallTable[lemming.AnimationFrame - 1];

        var orientation = lemming.Orientation;
        var levelPosition = lemming.LevelPosition;

        var gadgetEnumerator = Gadgets.GetAllGadgetsForPosition(levelPosition);

        while (gadgetEnumerator.MoveNext())
        {
            var gadget = gadgetEnumerator.Current;

            if (gadget.Type != GadgetType.Updraft)
                continue;
            if (!gadget.MatchesLemming(lemming))
                continue;

            if (gadget.Orientation == orientation.GetOpposite())
            {
                maxFallDistance--;
            }
        }

        var groundPixelDistance = Math.Max(FindGroundPixel(lemming, levelPosition), 0);
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

    public override LevelPosition GetAnchorPosition() => new(4, 16);

    protected override int TopLeftBoundsDeltaX() => -2;
    protected override int TopLeftBoundsDeltaY() => 11;

    protected override int BottomRightBoundsDeltaX() => 3;
}