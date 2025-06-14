namespace NeoLemmixSharp.IO.Data.Level.Objectives;

public abstract class ObjectiveCriterion
{
    public ObjectiveCriterionType Type { get; }

    protected ObjectiveCriterion(ObjectiveCriterionType type)
    {
        Type = type;
    }

    public abstract bool MatchesBaseCriterionData(ObjectiveCriterion other);
}

public sealed class SaveLemmingsCriterion : ObjectiveCriterion
{
    public required int SaveRequirement { get; init; }
    public required int TribeId { get; init; }

    public SaveLemmingsCriterion()
        : base(ObjectiveCriterionType.SaveLemmings)
    {
    }

    public override bool MatchesBaseCriterionData(ObjectiveCriterion other)
    {
        return other is SaveLemmingsCriterion otherSaveLemmingsCriterion && TribeId == otherSaveLemmingsCriterion.TribeId;
    }
}

public sealed class TimeLimitCriterion : ObjectiveCriterion
{
    public required int TimeLimitInSeconds { get; init; }

    public TimeLimitCriterion()
        : base(ObjectiveCriterionType.TimeLimit)
    {
    }

    public override bool MatchesBaseCriterionData(ObjectiveCriterion other)
    {
        return other is TimeLimitCriterion;
    }
}

public sealed class KillAllZombiesCriterion : ObjectiveCriterion
{
    public KillAllZombiesCriterion()
        : base(ObjectiveCriterionType.KillAllZombies)
    {
    }

    public override bool MatchesBaseCriterionData(ObjectiveCriterion other)
    {
        return other is KillAllZombiesCriterion;
    }
}

public enum ObjectiveCriterionType
{
    SaveLemmings,
    TimeLimit,
    KillAllZombies,
}
