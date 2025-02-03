using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class MinerSkill : LemmingSkill
{
    public static readonly MinerSkill Instance = new();

    private MinerSkill()
        : base(
            EngineConstants.MinerSkillId,
            EngineConstants.MinerSkillName)
    {
    }

    [SkipLocalsInit]
    public override bool CanAssignToLemming(Lemming lemming)
    {
        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        gadgetManager.GetAllGadgetsNearPosition(scratchSpaceSpan, lemming.LevelPosition, out var gadgetsNearRegion);

        return ActionIsAssignable(lemming) &&
               !PositionIsIndestructibleToLemming(
                   in gadgetsNearRegion,
                   lemming,
                   MinerAction.Instance,
                   lemming.Orientation.MoveRight(lemming.LevelPosition, lemming.FacingDirection.DeltaX));
    }

    public override void AssignToLemming(Lemming lemming)
    {
        MinerAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingActionHasher.CreateBitArraySet();

        result.Add(WalkerAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(PlatformerAction.Instance);
        result.Add(BuilderAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(FencerAction.Instance);
        result.Add(DiggerAction.Instance);
        result.Add(LasererAction.Instance);

        return result;
    }
}