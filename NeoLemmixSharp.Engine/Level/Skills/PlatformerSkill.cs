using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class PlatformerSkill : LemmingSkill
{
    public static readonly PlatformerSkill Instance = new();

    private PlatformerSkill()
        : base(
            EngineConstants.PlatformerSkillId,
            EngineConstants.PlatformerSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return ActionIsAssignable(lemming) &&
               PlatformerAction.LemmingCanPlatform(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        PlatformerAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateEmptySimpleSet();

        result.Add(WalkerAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(BuilderAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(FencerAction.Instance);
        result.Add(MinerAction.Instance);
        result.Add(DiggerAction.Instance);
        result.Add(LasererAction.Instance);

        return result;
    }
}