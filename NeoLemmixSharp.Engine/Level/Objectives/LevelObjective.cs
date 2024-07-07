﻿using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class LevelObjective
{
    private readonly IObjectiveRequirement[] _requirements;

    public int LevelObjectiveId { get; }
    public string LevelObjectiveTitle { get; }

    public ReadOnlySpan<IObjectiveRequirement> Requirements => new(_requirements);

    public ReadOnlySpan<SkillSetData> SkillSetData
    {
        get
        {
            foreach (var requirement in Requirements)
            {
                if (requirement is ISkillSetRequirement skillSetRequirement)
                    return skillSetRequirement.SkillSetData;
            }

            throw new InvalidOperationException("No skill set requirement has been specified");
        }
    }

    public LevelObjective(
        int levelObjectiveId,
        string levelObjectiveTitle,
        IObjectiveRequirement[] requirements)
    {
        LevelObjectiveId = levelObjectiveId;
        LevelObjectiveTitle = levelObjectiveTitle;
        _requirements = requirements;
    }

    public bool ObjectivesAreSatisfied()
    {
        var result = true;

        foreach (var requirement in _requirements)
        {
            result &= requirement.IsSatisfied;
        }

        return result;
    }

    public bool ObjectivesHaveBeenFailed()
    {
        var result = false;

        foreach (var requirement in _requirements)
        {
            result |= requirement.IsFailed;
        }

        return result;
    }
}