using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class DiggerSkill : LemmingSkill
{
    public static readonly DiggerSkill Instance = new();

    private DiggerSkill()
        : base(
            LevelConstants.DiggerSkillId,
            LevelConstants.DiggerSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        LevelScreen.GadgetManager.GetAllGadgetsForPosition(lemming.LevelPosition, out var gadgetsNearRegion);

        return ActionIsAssignable(lemming) &&
               !PositionIsIndestructibleToLemming(in gadgetsNearRegion, lemming, DiggerAction.Instance, lemming.LevelPosition);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        DiggerAction.Instance.TransitionLemmingToAction(lemming, false);
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
        yield return LasererAction.Instance;
    }
}