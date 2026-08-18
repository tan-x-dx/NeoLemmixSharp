using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class BuilderSkill : LemmingSkill
{
    public static readonly BuilderSkill Instance = new();

    private BuilderSkill()
        : base(
            LemmingSkillType.BuilderSkill,
            LemmingSkillConstants.BuilderSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        BuilderAction.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(LemmingActionType.WalkerAction);
        result.Add(LemmingActionType.ShruggerAction);
        result.Add(LemmingActionType.PlatformerAction);
        result.Add(LemmingActionType.StackerAction);
        result.Add(LemmingActionType.LasererAction);
        result.Add(LemmingActionType.BasherAction);
        result.Add(LemmingActionType.FencerAction);
        result.Add(LemmingActionType.MinerAction);
        result.Add(LemmingActionType.DiggerAction);

        return result;
    }
}
