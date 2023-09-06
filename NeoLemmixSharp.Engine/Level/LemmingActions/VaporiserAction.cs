using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class VaporiserAction : LemmingAction
{
    public static VaporiserAction Instance { get; } = new();

    private VaporiserAction()
    {
    }

    public override int Id => GameConstants.VaporiserActionId;
    public override string LemmingActionName => "burner2";
    public override int NumberOfAnimationFrames => GameConstants.VaporiserAnimationFrames;
    public override bool IsOneTimeAction => true;
    public override int CursorSelectionPriorityValue => GameConstants.NoPriority;

    public override bool UpdateLemming(Lemming lemming)
    {
        if (lemming.EndOfAnimation)
        {
            // ?? RemoveLemming(L, RM_KILL); ??
        }

        return false;
    }

    public override LevelPosition GetAnchorPosition() => new(5, 14);

    protected override int TopLeftBoundsDeltaX() => -2;
    protected override int TopLeftBoundsDeltaY() => 12;

    protected override int BottomRightBoundsDeltaX() => 3;
}