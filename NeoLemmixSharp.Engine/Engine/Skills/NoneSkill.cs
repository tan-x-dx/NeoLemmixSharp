using NeoLemmixSharp.Engine.Engine.LemmingActions;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public sealed class NoneSkill : LemmingSkill
{
    /// <summary>
    /// Logically equivalent to null, but null references suck.
    /// </summary>
    public static NoneSkill Instance { get; } = new();

    private NoneSkill()
    {
    }

    public override int Id => -1;
    public override string LemmingSkillName => "none";
    public override bool IsPermanentSkill => false;
    public override bool IsClassicSkill => false;

    public override bool CanAssignToLemming(Lemming lemming)
    {
        return false;
    }

    public override bool AssignToLemming(Lemming lemming)
    {
        return false;
    }

    protected override IEnumerable<LemmingAction> ActionsThatCanBeAssigned() => Enumerable.Empty<LemmingAction>();
}