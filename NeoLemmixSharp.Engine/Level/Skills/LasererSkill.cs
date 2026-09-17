using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class LasererSkill
{
    public static bool CanAssignToLemming(Lemming lemming)
    {
        return LemmingSkillType.LasererSkill.SkillIsAssignableToCurrentAction(lemming.CurrentActionType);
    }

    public static void AssignToLemming(Lemming lemming)
    {
        LasererAction.TransitionLemmingToAction(lemming, false);
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssigned()
    {
        yield return LemmingActionType.WalkerAction;
        yield return LemmingActionType.ShruggerAction;
        yield return LemmingActionType.PlatformerAction;
        yield return LemmingActionType.BuilderAction;
        yield return LemmingActionType.StackerAction;
        yield return LemmingActionType.BasherAction;
        yield return LemmingActionType.FencerAction;
        yield return LemmingActionType.MinerAction;
        yield return LemmingActionType.DiggerAction;
    }
}
