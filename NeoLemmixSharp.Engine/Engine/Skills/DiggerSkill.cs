using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class DiggerSkill : LemmingSkill
{
    public static DiggerSkill Instance { get; } = new();

    private DiggerSkill()
    {
    }

    public override int Id => GameConstants.DiggerSkillId;
    public override string LemmingSkillName => "digger";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return ActionIsAssignable(lemming) &&
               !Terrain.PixelIsIndestructibleToLemming(lemming, DiggerAction.Instance, lemming.LevelPosition);
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        DiggerAction.Instance.TransitionLemmingToAction(lemming, false);
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
        yield return LasererAction.Instance;
    }
}