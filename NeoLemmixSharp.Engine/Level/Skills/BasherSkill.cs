﻿using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class BasherSkill : LemmingSkill
{
    public static BasherSkill Instance { get; } = new();

    private BasherSkill()
    {
    }

    public override int Id => Global.BasherSkillId;
    public override string LemmingSkillName => "basher";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => true;

    public override bool AssignToLemming(Lemming lemming)
    {
        BasherAction.Instance.TransitionLemmingToAction(lemming, false);
        return true;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield return WalkerAction.Instance;
        yield return ShruggerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return BuilderAction.Instance;
        yield return StackerAction.Instance;
        yield return FencerAction.Instance;
        yield return MinerAction.Instance;
        yield return DiggerAction.Instance;
        yield return LasererAction.Instance;
    }
}