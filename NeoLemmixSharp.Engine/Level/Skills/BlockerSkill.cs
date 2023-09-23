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

    public override int Id => GameConstants.BlockerSkillId;
    public override string LemmingSkillName => "blocker";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => true;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        if (!ActionIsAssignable(lemming))
            return false;

        var firstBounds = BlockerAction.Instance.GetLemmingBounds(lemming);

        var topLeft = firstBounds.GetTopLeftPosition();
        var bottomRight = firstBounds.GetBottomRightPosition();

        var nearbyBlockers = LemmingManager.GetAllBlockersNearLemming(topLeft, bottomRight);

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