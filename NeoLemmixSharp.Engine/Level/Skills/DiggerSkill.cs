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

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateEmptySimpleSet();

        result.Add(WalkerAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(PlatformerAction.Instance);
        result.Add(BuilderAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(FencerAction.Instance);
        result.Add(MinerAction.Instance);
        result.Add(LasererAction.Instance);

        return result;
    }
}