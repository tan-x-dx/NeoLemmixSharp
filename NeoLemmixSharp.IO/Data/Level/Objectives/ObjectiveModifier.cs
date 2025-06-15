namespace NeoLemmixSharp.IO.Data.Level.Objectives;

public abstract class ObjectiveModifier
{
    public ObjectiveModifierType Type { get; }

    public ObjectiveModifier(ObjectiveModifierType type)
    {
        Type = type;
    }

    public abstract bool MatchesBaseModifierData(ObjectiveModifier other);
}

public sealed class LimitSpecificSkillAssignmentsModifier : ObjectiveModifier
{
    public required int SkillId { get; init; }
    public required int TribeId { get; init; }
    public required int MaxSkillAssignments { get; init; }

    public LimitSpecificSkillAssignmentsModifier()
        : base(ObjectiveModifierType.LimitSpecificSkillAssignments)
    {
    }

    public override bool MatchesBaseModifierData(ObjectiveModifier other)
    {
        return other is LimitSpecificSkillAssignmentsModifier otherLimitSpecificSkillAssignmentsModifier &&
               SkillId == otherLimitSpecificSkillAssignmentsModifier.SkillId &&
               TribeId == otherLimitSpecificSkillAssignmentsModifier.TribeId;
    }
}

public sealed class LimitTotalSkillAssignmentsModifier : ObjectiveModifier
{
    public required int MaxTotalSkillAssignments { get; init; }

    public LimitTotalSkillAssignmentsModifier()
        : base(ObjectiveModifierType.LimitTotalSkillAssignments)
    {
    }

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
