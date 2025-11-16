namespace NeoLemmixSharp.IO.Data.Level.Objectives;

public abstract class ObjectiveCriterionData
{
    public ObjectiveCriterionType Type { get; }

    protected ObjectiveCriterionData(ObjectiveCriterionType type)
    {
        Type = type;
    }

    public abstract bool MatchesBaseCriterionData(ObjectiveCriterionData other);
}

public sealed class SaveLemmingsCriterionData : ObjectiveCriterionData
{
    public required int SaveRequirement { get; init; }
    public required int TribeId { get; init; }

    public SaveLemmingsCriterionData()
        : base(ObjectiveCriterionType.SaveLemmings)
    {
    }

    public override bool MatchesBaseCriterionData(ObjectiveCriterionData other)
    {
        return other is SaveLemmingsCriterionData otherSaveLemmingsCriterion && TribeId == otherSaveLemmingsCriterion.TribeId;
    }
}

public sealed class TimeLimitCriterionData : ObjectiveCriterionData
{
    public required uint TimeLimitInSeconds { get; init; }

    public TimeLimitCriterionData()
        : base(ObjectiveCriterionType.TimeLimit)
    {
    }

    public override bool MatchesBaseCriterionData(ObjectiveCriterionData other)
    {
        return other is TimeLimitCriterionData;
    }
}

public sealed class KillAllZombiesCriterionData : ObjectiveCriterionData
{
    public KillAllZombiesCriterionData()
        : base(ObjectiveCriterionType.KillAllZombies)
    {
    }

    public override bool MatchesBaseCriterionData(ObjectiveCriterionData other)
    {
        return other is KillAllZombiesCriterionData;
    }
}
