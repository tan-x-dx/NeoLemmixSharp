using NeoLemmixSharp.Engine.Engine.LemmingActions;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class ShimmierSkill : LemmingSkill
{
    public static ShimmierSkill Instance { get; } = new();

    private ShimmierSkill()
    {
    }

    public override int Id => GameConstants.ShimmierSkillId;
    public override string LemmingSkillName => "shimmier";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool AssignToLemming(Lemming lemming)
    {
        if (lemming.CurrentAction == ClimberAction.Instance ||
            lemming.CurrentAction == SliderAction.Instance ||
            lemming.CurrentAction == JumperAction.Instance ||
            lemming.CurrentAction == DehoisterAction.Instance)
        {
            ShimmierAction.Instance.TransitionLemmingToAction(lemming, false);
        }
        else
        {
            ReacherAction.Instance.TransitionLemmingToAction(lemming, false);
        }

        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return BuilderAction.Instance;
        yield return StackerAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return LasererAction.Instance;
    }
}