using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class StonerSkill : LemmingSkill
{
    public static readonly StonerSkill Instance = new();

    private StonerSkill()
        : base(
            LemmingSkillType.StonerSkill,
            LemmingSkillConstants.StonerSkillName)
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

        lemming.SetCountDownAction(countDownTimer, LemmingActionType.StonerAction, displayTimer);
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned() => ActionsThatCanBeAssignedPermanentSkill;
}
