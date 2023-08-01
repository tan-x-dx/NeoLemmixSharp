using NeoLemmixSharp.Engine.Engine.Actions;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class JumperSkill : LemmingSkill
{
    public static JumperSkill Instance { get; } = new();

    private JumperSkill()
    {
    }

    public override int Id => GameConstants.JumperSkillId;
    public override string LemmingSkillName => "jumper";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;
    
    public override bool AssignToLemming(Lemming lemming)
    {
        JumperAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return DiggerAction.Instance;
        yield return BuilderAction.Instance;
        yield return BasherAction.Instance;
        yield return MinerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return StackerAction.Instance;
        yield return FencerAction.Instance;
        yield return ClimberAction.Instance;
        yield return SliderAction.Instance;
        yield return LasererAction.Instance;
    }
}