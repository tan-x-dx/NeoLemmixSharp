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
            -1,
            LevelConstants.NoneSkillName)
    {
    }

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return false;
    }

    public override void AssignToLemming(Lemming lemming)
    {
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned()
    {
        yield break;
    }
}