using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class BlockerSkill : LemmingSkill
{
    public static BlockerSkill Instance { get; } = new();

    private BlockerSkill()
    {
    }

    public override int Id => Global.BlockerSkillId;
    public override string LemmingSkillName => "blocker";
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        if (!ActionIsAssignable(lemming))
            return false;

        var firstBounds = BlockerAction.Instance.GetLemmingBounds(lemming);

        var nearbyBlockers = Global.LemmingManager.GetAllBlockersNearLemming(firstBounds);

        foreach (var blocker in nearbyBlockers)
        {
            var blockerTopLeft = blocker.TopLeftPixel;
            var blockerBottomRight = blocker.BottomRightPixel;

            var secondBounds = new LevelPositionPair(blockerTopLeft, blockerBottomRight);

            if (firstBounds.Overlaps(secondBounds))
                return false;
        }

        return true;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        BlockerAction.Instance.TransitionLemmingToAction(lemming, false);
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