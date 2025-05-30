using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class BuilderSkill : LemmingSkill
{
    public static readonly BuilderSkill Instance = new();

    private BuilderSkill()
        : base(
            LemmingSkillConstants.BuilderSkillId,
            LemmingSkillConstants.BuilderSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        BuilderAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(WalkerAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(PlatformerAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(LasererAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(FencerAction.Instance);
        result.Add(MinerAction.Instance);
        result.Add(DiggerAction.Instance);

        return result;
    }
}