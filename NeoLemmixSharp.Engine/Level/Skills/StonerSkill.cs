using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class StonerSkill : LemmingSkill
{
    public static readonly StonerSkill Instance = new();

    private StonerSkill()
        : base(
            LemmingSkillConstants.StonerSkillId,
            LemmingSkillConstants.StonerSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return lemming.Data.CountDownTimer == 0 && SkillIsAssignableToCurrentAction(lemming);
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