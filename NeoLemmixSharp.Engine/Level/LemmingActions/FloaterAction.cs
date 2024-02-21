using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class FloaterAction : LemmingAction
{
    public static readonly FloaterAction Instance = new();

    private readonly int[] _floaterFallTable =
    [
        3,
        3,
        3,
        3,
        -1,
        0,
        1,
        1,
        2,
        2,
        2,
        2,
        2,
        2,
        2,
        2,
        2
    ];

    private FloaterAction()
    {
    }

    public override int Id => LevelConstants.FloaterActionId;
    public override string LemmingActionName => "floater";
    public override int NumberOfAnimationFrames => LevelConstants.FloaterAnimationFrames;
    public override bool IsOneTimeAction => false;
    public override int CursorSelectionPriorityValue => LevelConstants.PermanentSkillPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        var maxFallDistance = _floaterFallTable[lemming.PhysicsFrame - 1];

        var orientation = lemming.Orientation;
        var levelPosition = lemming.LevelPosition;

        var gadgetSet = LevelScreen.GadgetManager.GetAllGadgetsAtLemmingPosition(lemming);
        if (gadgetSet.Count > 0)
        {
            foreach (var gadget in gadgetSet)
            {
                if (gadget.GadgetBehaviour != UpdraftGadgetBehaviour.Instance || !gadget.MatchesLemming(lemming))
                    continue;

                if (gadget.Orientation == Orientation.GetOpposite(orientation))
                {
                    maxFallDistance--;
                }
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

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 12;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 4;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => 1;
}