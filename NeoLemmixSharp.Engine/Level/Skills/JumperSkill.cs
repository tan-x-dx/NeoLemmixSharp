using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class JumperSkill
{
    public static bool CanAssignToLemming(Lemming lemming)
    {
        return LemmingSkillType.JumperSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        JumperAction.TransitionLemmingToAction(lemming, false);
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned()
    {
        yield return LemmingActionType.WalkerAction;
        yield return LemmingActionType.DiggerAction;
        yield return LemmingActionType.BuilderAction;
        yield return LemmingActionType.BasherAction;
        yield return LemmingActionType.MinerAction;
        yield return LemmingActionType.ShruggerAction;
        yield return LemmingActionType.PlatformerAction;
        yield return LemmingActionType.StackerAction;
        yield return LemmingActionType.FencerAction;
        yield return LemmingActionType.ClimberAction;
        yield return LemmingActionType.SliderAction;
        yield return LemmingActionType.LasererAction;
    }
}
