﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Runtime.CompilerServices;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class ShimmierSkill : LemmingSkill
{
    public static readonly ShimmierSkill Instance = new();

    private ShimmierSkill()
        : base(
            EngineConstants.ShimmierSkillId,
            EngineConstants.ShimmierSkillName)
    {
    }

    [SkipLocalsInit]
    public override bool CanAssignToLemming(Lemming lemming)
    {
        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        if (lemming.CurrentAction == ClimberAction.Instance)
        {
            var simulationLemming = LemmingManager.SimulateLemming(lemming, true);

            if (simulationLemming.CurrentAction != SliderAction.Instance &&
                (simulationLemming.CurrentAction != FallerAction.Instance ||
                 simulationLemming.FacingDirection != lemming.FacingDirection.GetOpposite()))
                return false;

            var simulationOrientation = simulationLemming.Orientation;
            var simulationPosition = simulationLemming.LevelPosition;

            var gadgetTestRegion = new LevelRegion(
                simulationPosition,
                simulationOrientation.MoveUp(simulationPosition, 9));
            gadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, gadgetTestRegion, out var gadgetsNearRegion);

            return PositionIsSolidToLemming(in gadgetsNearRegion, simulationLemming, simulationOrientation.MoveUp(simulationPosition, 9)) ||
                   PositionIsSolidToLemming(in gadgetsNearRegion, simulationLemming, simulationOrientation.MoveUp(simulationPosition, 8));
        }

        if (lemming.CurrentAction == SliderAction.Instance ||
            lemming.CurrentAction == DehoisterAction.Instance)
        {
            var oldAction = lemming.CurrentAction;

            var simulationLemming = LemmingManager.SimulateLemming(lemming, true);

            return simulationLemming.CurrentAction != oldAction &&
                   simulationLemming.FacingDirection == lemming.FacingDirection &&
                   (oldAction != DehoisterAction.Instance || simulationLemming.CurrentAction != SliderAction.Instance);
        }

        if (lemming.CurrentAction != JumperAction.Instance)
            return ActionIsAssignable(lemming);

        var orientation = lemming.Orientation;
        var lemmingPosition = lemming.LevelPosition;

        var gadgetTestRegion1 = new LevelRegion(
            lemmingPosition,
            orientation.MoveUp(lemmingPosition, 12));
        gadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, gadgetTestRegion1, out var gadgetsNearRegion1);

        for (var i = -1; i < 4; i++)
        {
            if (PositionIsSolidToLemming(in gadgetsNearRegion1, lemming, orientation.MoveUp(lemmingPosition, 9 + i)) &&
                !PositionIsSolidToLemming(in gadgetsNearRegion1, lemming, orientation.MoveUp(lemmingPosition, 8 + i)))
                return true;
        }

        return ActionIsAssignable(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        LemmingAction nextAction = lemming.CurrentAction == ClimberAction.Instance ||
                                   lemming.CurrentAction == SliderAction.Instance ||
                                   lemming.CurrentAction == JumperAction.Instance ||
                                   lemming.CurrentAction == DehoisterAction.Instance
            ? ShimmierAction.Instance
            : ReacherAction.Instance;

        nextAction.TransitionLemmingToAction(lemming, false);
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(WalkerAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(PlatformerAction.Instance);
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