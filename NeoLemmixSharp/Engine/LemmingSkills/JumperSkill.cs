﻿using NeoLemmixSharp.Engine.LemmingActions;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public sealed class JumperSkill : LemmingSkill
{
    public static JumperSkill Instance { get; } = new();

    private JumperSkill()
    {
    }

    public override int LemmingSkillId => 11;
    public override string LemmingSkillName => "jumper";
    public override bool IsPermanentSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.CurrentAction == WalkerAction.Instance ||
               lemming.CurrentAction == DiggerAction.Instance ||
               lemming.CurrentAction == BuilderAction.Instance ||
               lemming.CurrentAction == BasherAction.Instance ||
               lemming.CurrentAction == MinerAction.Instance ||
               lemming.CurrentAction == ShruggerAction.Instance ||
               lemming.CurrentAction == PlatformerAction.Instance ||
               lemming.CurrentAction == StackerAction.Instance ||
               lemming.CurrentAction == FencerAction.Instance ||
               lemming.CurrentAction == ClimberAction.Instance ||
               lemming.CurrentAction == SliderAction.Instance ||
               lemming.CurrentAction == LasererAction.Instance;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        JumperAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }
}