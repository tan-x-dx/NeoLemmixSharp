﻿using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class BomberSkill : LemmingSkill
{
    public static BomberSkill Instance { get; } = new();

    private BomberSkill()
    {
    }

    public override int Id => GameConstants.BomberSkillId;
    public override string LemmingSkillName => "bomber";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => true;

    public override bool AssignToLemming(Lemming lemming)
    {
        /*
            lemming.ExplosionTimer : = 1;
            lemming.TimerToStone : = False;
            lemming.HideCountdown : = True;
            */

        throw new NotImplementedException();
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill();
}