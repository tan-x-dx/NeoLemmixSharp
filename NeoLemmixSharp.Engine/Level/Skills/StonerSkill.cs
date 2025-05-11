﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class StonerSkill : LemmingSkill
{
    public static readonly StonerSkill Instance = new();

    private StonerSkill()
        : base(
            EngineConstants.StonerSkillId,
            EngineConstants.StonerSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CountDownTimer == 0 && SkillIsAssignableToCurrentAction(lemming);
    }

    public override void AssignToLemming(Lemming lemming)
    {
        var levelParameters = LevelScreen.LevelParameters;
        var countDownTimer = levelParameters.GetLemmingCountDownTimer(lemming);
        var displayTimer = levelParameters.Contains(LevelParameters.TimedBombers);

        lemming.SetCountDownAction(countDownTimer, StonerAction.Instance, displayTimer);
    }

    protected override LemmingActionSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;
}