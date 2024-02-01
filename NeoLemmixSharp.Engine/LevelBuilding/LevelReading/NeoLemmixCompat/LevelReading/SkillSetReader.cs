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
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);

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
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

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