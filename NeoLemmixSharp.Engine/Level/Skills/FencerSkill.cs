using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class FencerSkill : LemmingSkill
{
    public static readonly FencerSkill Instance = new();

    private FencerSkill()
        : base(
            LemmingSkillType.FencerSkill,
            LemmingSkillConstants.FencerSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        FencerAction.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(LemmingActionType.WalkerAction);
        result.Add(LemmingActionType.ShruggerAction);
        result.Add(LemmingActionType.PlatformerAction);
        result.Add(LemmingActionType.BuilderAction);
        result.Add(LemmingActionType.StackerAction);
        result.Add(LemmingActionType.BasherAction);
        result.Add(LemmingActionType.MinerAction);
        result.Add(LemmingActionType.DiggerAction);
        result.Add(LemmingActionType.LasererAction);

        return result;
    }
}
