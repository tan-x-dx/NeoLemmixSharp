﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class PlatformerSkill : LemmingSkill
{
    public static readonly PlatformerSkill Instance = new();

    private PlatformerSkill()
        : base(
            LemmingSkillConstants.PlatformerSkillId,
            LemmingSkillConstants.PlatformerSkillName)
    {
    }

    [SkipLocalsInit]
    public override bool CanAssignToLemming(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var lemmingPostion = lemming.AnchorPosition;

        var levelRegion = new RectangularRegion(
            orientation.Move(lemmingPostion, 5, 2),
            orientation.Move(lemmingPostion, -5, -2));
        Span<uint> scratchSpaceSpan = stackalloc uint[LevelScreen.GadgetManager.ScratchSpaceSize];
        LevelScreen.GadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, levelRegion, out var gadgetsNearLemming);

        return SkillIsAssignableToCurrentAction(lemming) &&
               PlatformerAction.LemmingCanPlatform(lemming, in gadgetsNearLemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        PlatformerAction.Instance.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(WalkerAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(BuilderAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(FencerAction.Instance);
        result.Add(MinerAction.Instance);
        result.Add(DiggerAction.Instance);
        result.Add(LasererAction.Instance);

        return result;
    }
}