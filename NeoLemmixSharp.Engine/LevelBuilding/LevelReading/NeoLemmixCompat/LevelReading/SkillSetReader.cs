using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class SkillSetReader : INeoLemmixDataReader
{
    private readonly CaseInvariantCharEqualityComparer _charEqualityComparer = new();
    private readonly List<SkillSetData> _skillSetData = new();

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$SKILLSET";

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        ReadingHelpers.GetTokenPair(line, out var firstToken, out _, out _);

        if (firstToken is "$END")
        {
            FinishedReading = true;
            return false;
        }

        ReadSkillSetData(line);
        return false;
    }

    private void ReadSkillSetData(ReadOnlySpan<char> line)
    {
        ReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

        if (!ReadingHelpers.GetSkillByName(firstToken, _charEqualityComparer, out var skill))
            throw new Exception($"Unknown token: {firstToken}");

        var amount = secondToken is "INFINITE"
            ? LevelConstants.InfiniteSkillCount
            : int.Parse(secondToken);

        var skillSetDatum = new SkillSetData
        {
            Skill = skill,
            NumberOfSkills = amount,
            TeamId = 0
        };

        _skillSetData.Add(skillSetDatum);
    }

    public void ApplyToLevelData(LevelData levelData)
    {
        levelData.SkillSetData.AddRange(_skillSetData);
    }

    public void Dispose()
    {
        _skillSetData.Clear();
    }
}