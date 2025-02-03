using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class FencerSkill : LemmingSkill
{
    public static readonly FencerSkill Instance = new();

    private FencerSkill()
        : base(
            EngineConstants.FencerSkillId,
            EngineConstants.FencerSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        FencerAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingActionHasher.CreateSimpleSet();

        result.Add(WalkerAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(PlatformerAction.Instance);
        result.Add(BuilderAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(MinerAction.Instance);
        result.Add(DiggerAction.Instance);
        result.Add(LasererAction.Instance);

        return result;
    }
}