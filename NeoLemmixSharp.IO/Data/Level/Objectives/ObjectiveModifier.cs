namespace NeoLemmixSharp.IO.Data.Level.Objectives;

public abstract class ObjectiveModifier
{
    public abstract bool MatchesBaseModifierData(ObjectiveModifier other);
}

public sealed class LimitSpecificSkillAssignmentsModifier : ObjectiveModifier
{
    public required int SkillId { get; init; }
    public required int MaxSkillAssignments { get; init; }

    public override bool MatchesBaseModifierData(ObjectiveModifier other)
    {
        return other is LimitSpecificSkillAssignmentsModifier otherLimitSpecificSkillAssignmentsModifier &&
               SkillId == otherLimitSpecificSkillAssignmentsModifier.SkillId;
    }
}

public sealed class LimitTotalSkillAssignmentsModifier : ObjectiveModifier
{
    public required int MaxTotalSkillAssignments { get; init; }

    public override bool MatchesBaseModifierData(ObjectiveModifier other)
    {
        return other is LimitTotalSkillAssignmentsModifier;
    }
}

public enum ObjectiveModifierType
{
    LimitSpecificSkillAssignments,
    LimitTotalSkillAssignments,
}
