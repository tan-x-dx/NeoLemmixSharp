using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class JumperSkill : LemmingSkill
{
    public static readonly JumperSkill Instance = new();

    private JumperSkill()
        : base(
            LemmingSkillType.JumperSkill,
            LemmingSkillConstants.JumperSkillName)
    {
    }

    public override void AssignToLemming(Lemming lemming)
    {
        JumperAction.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(LemmingActionType.WalkerAction);
        result.Add(LemmingActionType.DiggerAction);
        result.Add(LemmingActionType.BuilderAction);
        result.Add(LemmingActionType.BasherAction);
        result.Add(LemmingActionType.MinerAction);
        result.Add(LemmingActionType.ShruggerAction);
        result.Add(LemmingActionType.PlatformerAction);
        result.Add(LemmingActionType.StackerAction);
        result.Add(LemmingActionType.FencerAction);
        result.Add(LemmingActionType.ClimberAction);
        result.Add(LemmingActionType.SliderAction);
        result.Add(LemmingActionType.LasererAction);

        return result;
    }
}
