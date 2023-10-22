using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class SkillSetReader : INeoLemmixDataReader
{
    private readonly Dictionary<string, LemmingSkill> _skillLookup = new();
    private readonly List<SkillSetData> _skillSetData;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$SKILLSET";

    public SkillSetReader(LevelData levelData)
    {
        _skillSetData = levelData.SkillSetData;

        foreach (var lemmingSkill in LemmingSkill.AllItems)
        {
            var name = lemmingSkill.LemmingSkillName.ToUpperInvariant();
            _skillLookup.Add(name, lemmingSkill);
        }
    }

    public void BeginReading(string[] tokens)
    {
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        if (tokens[0] == "$END")
        {
            FinishedReading = true;
            return;
        }

        ReadSkillSetData(tokens);
    }

    private void ReadSkillSetData(string[] tokens)
    {
        if (!_skillLookup.TryGetValue(tokens[0], out var skill))
            throw new Exception($"Unknown token: {tokens[0]}");

        var amount = tokens[1] == "INFINITE" 
            ? 100
            : ReadingHelpers.ReadInt(tokens[1]);

        var skillSetDatum = new SkillSetData
        {
            Skill = skill,
            NumberOfSkills = amount,
            TeamId = 0
        };

        _skillSetData.Add(skillSetDatum);
    }
}