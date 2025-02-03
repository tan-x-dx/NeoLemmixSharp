using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class JumperSkill : LemmingSkill
{
    public static readonly JumperSkill Instance = new();

    private JumperSkill()
        : base(
            EngineConstants.JumperSkillId,
            EngineConstants.JumperSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        JumperAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingActionComparer.CreateSimpleSet();

        result.Add(WalkerAction.Instance);
        result.Add(DiggerAction.Instance);
        result.Add(BuilderAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(MinerAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(PlatformerAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(FencerAction.Instance);
        result.Add(ClimberAction.Instance);
        result.Add(SliderAction.Instance);
        result.Add(LasererAction.Instance);

        return result;
    }
}