namespace NeoLemmixSharp.IO.Data.Level.Objectives;

public abstract class ObjectiveModifierData
{
    public ObjectiveModifierType Type { get; }

    public ObjectiveModifierData(ObjectiveModifierType type)
    {
        Type = type;
    }

    public abstract bool MatchesBaseModifierData(ObjectiveModifierData other);
}

public sealed class LimitSpecificSkillAssignmentsModifierData : ObjectiveModifierData
{
    public required int SkillId { get; init; }
    public required int TribeId { get; init; }
    public required int MaxSkillAssignments { get; init; }

    public LimitSpecificSkillAssignmentsModifierData()
        : base(ObjectiveModifierType.LimitSpecificSkillAssignments)
    {
    }

    public override bool MatchesBaseModifierData(ObjectiveModifierData other)
    {
        return other is LimitSpecificSkillAssignmentsModifierData otherLimitSpecificSkillAssignmentsModifier &&
               SkillId == otherLimitSpecificSkillAssignmentsModifier.SkillId &&
               TribeId == otherLimitSpecificSkillAssignmentsModifier.TribeId;
    }
}

public sealed class LimitTotalSkillAssignmentsModifierData : ObjectiveModifierData
{
    public required int MaxTotalSkillAssignments { get; init; }

    public LimitTotalSkillAssignmentsModifierData()
        : base(ObjectiveModifierType.LimitTotalSkillAssignments)
    {
    }

    public override bool MatchesBaseModifierData(ObjectiveModifierData other)
    {
        return other is LimitTotalSkillAssignmentsModifierData;
    }
}

public enum ObjectiveModifierType
{
    LimitSpecificSkillAssignments,
    LimitTotalSkillAssignments,
}
