using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class PlatformerSkill : LemmingSkill
{
    public static readonly PlatformerSkill Instance = new();

    private PlatformerSkill()
        : base(
            LemmingSkillType.PlatformerSkill,
            LemmingSkillConstants.PlatformerSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPostion = lemming.AnchorPosition;

        var levelRegion = new RectangularRegion(
            orientation.Move(lemmingPostion, new(-5, 2)),
            orientation.Move(lemmingPostion, new(5, -2)));
        LevelScreen.GadgetManager.GetAllItemsNearRegion(levelRegion, out var gadgetsNearLemming);

        return SkillIsAssignableToCurrentAction(lemming) &&
               PlatformerAction.LemmingCanPlatform(lemming, in gadgetsNearLemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        PlatformerAction.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(LemmingActionType.WalkerAction);
        result.Add(LemmingActionType.ShruggerAction);
        result.Add(LemmingActionType.BuilderAction);
        result.Add(LemmingActionType.StackerAction);
        result.Add(LemmingActionType.BasherAction);
        result.Add(LemmingActionType.FencerAction);
        result.Add(LemmingActionType.MinerAction);
        result.Add(LemmingActionType.DiggerAction);
        result.Add(LemmingActionType.LasererAction);

        return result;
    }
}
