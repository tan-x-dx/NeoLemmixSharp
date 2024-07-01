using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class LevelObjective
{
    private readonly IObjectiveRequirement[] _requirements;
    private readonly List<SkillSetData> _skillSetData;

    public int LevelObjectiveId { get; }
    public string LevelObjectiveTitle { get; }

    public ReadOnlySpan<IObjectiveRequirement> Requirements => new(_requirements);
    public ReadOnlySpan<SkillSetData> SkillSetData => CollectionsMarshal.AsSpan(_skillSetData);

    public LevelObjective(
        int levelObjectiveId,
        string levelObjectiveTitle,
        IObjectiveRequirement[] requirements,
        List<SkillSetData> skillSetData)
    {
        LevelObjectiveId = levelObjectiveId;
        LevelObjectiveTitle = levelObjectiveTitle;
        _requirements = requirements;
        _skillSetData = skillSetData;
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