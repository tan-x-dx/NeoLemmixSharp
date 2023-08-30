using NeoLemmixSharp.Engine.Engine.LemmingActions;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class StackerSkill : LemmingSkill
{
    public static StackerSkill Instance { get; } = new();

    private StackerSkill()
    {
    }

    public override int Id => GameConstants.StackerSkillId;
    public override string LemmingSkillName => "stacker";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool AssignToLemming(Lemming lemming)
    {
        // Get starting position for stacker
        lemming.StackLow = !Terrain.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX));

        StackerAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return BuilderAction.Instance;
        yield return BasherAction.Instance;
        yield return FencerAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return LasererAction.Instance;
    }
}