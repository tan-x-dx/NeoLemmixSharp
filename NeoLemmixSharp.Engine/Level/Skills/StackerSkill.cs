using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class StackerSkill : LemmingSkill
{
    public static readonly StackerSkill Instance = new();

    private StackerSkill()
        : base(
            LemmingSkillConstants.StackerSkillId,
            LemmingSkillConstants.StackerSkillName)
    {
    }

    [SkipLocalsInit]
    public override void AssignToLemming(Lemming lemming)
    {
        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        // Get starting position for stacker
        gadgetManager.GetAllGadgetsNearPosition(
            scratchSpaceSpan,
            lemming.Orientation.MoveRight(lemming.AnchorPosition, lemming.FacingDirection.DeltaX),
            out var gadgetsNearRegion);

        lemming.StackLow = !PositionIsSolidToLemming(
            in gadgetsNearRegion,
            lemming,
            lemming.Orientation.MoveRight(lemming.AnchorPosition, lemming.FacingDirection.DeltaX));

        StackerAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(WalkerAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(PlatformerAction.Instance);
        result.Add(BuilderAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(FencerAction.Instance);
        result.Add(MinerAction.Instance);
        result.Add(DiggerAction.Instance);
        result.Add(LasererAction.Instance);

        return result;
    }
}