using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class DiggerSkill : LemmingSkill
{
    public static readonly DiggerSkill Instance = new();

    private DiggerSkill()
        : base(
            EngineConstants.DiggerSkillId,
            EngineConstants.DiggerSkillName)
    {
    }

    [SkipLocalsInit]
    public override bool CanAssignToLemming(Lemming lemming)
    {
        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        gadgetManager.GetAllGadgetsForPosition(scratchSpaceSpan, lemming.LevelPosition, out var gadgetsNearRegion);

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