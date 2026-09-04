using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public sealed class NoneSkill : LemmingSkill
{
    /// <summary>
    /// Logically equivalent to null, but null references suck.
    /// </summary>
    public static readonly NoneSkill Instance = new();

    private NoneSkill()
        : base(
            LemmingSkillType.NoneSkill,
            LemmingSkillConstants.NoneSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return false;
    }

    public override void AssignToLemming(Lemming lemming)
    {
    }

    protected override LemmingActionTypeSet ActionsThatCanBeAssigned() => LemmingAction.CreateBitArraySet();
}
