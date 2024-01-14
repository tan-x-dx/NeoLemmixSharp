using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class SkillSetReader : INeoLemmixDataReader, IEqualityComparer<char>
{
    private readonly List<SkillSetData> _skillSetData;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$SKILLSET";

    public SkillSetReader(LevelData levelData)
    {
        _skillSetData = levelData.SkillSetData;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
    }

    public void ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);

        if (firstToken is "$END")
        {
            FinishedReading = true;
            return;
        }

        ReadSkillSetData(line);
    }

    private void ReadSkillSetData(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

        if (!GetSkillByName(firstToken, out var skill))
            throw new Exception($"Unknown token: {firstToken}");

        var amount = secondToken is "INFINITE"
            ? LevelConstants.InfiniteSkillCount
            : ReadingHelpers.ReadInt(secondToken);

        var skillSetDatum = new SkillSetData
        {
            Skill = skill,
            NumberOfSkills = amount,
            TeamId = 0
        };

        _skillSetData.Add(skillSetDatum);
    }

    private bool GetSkillByName(ReadOnlySpan<char> token, out LemmingSkill lemmingSkill)
    {
        foreach (var item in LemmingSkill.AllItems)
        {
            var skillName = item.LemmingSkillName.AsSpan();
            if (skillName.SequenceEqual(token, this))
            {
                lemmingSkill = item;
                return true;
            }
        }

        lemmingSkill = null!;
        return false;
    }

    bool IEqualityComparer<char>.Equals(char x, char y)
    {
        return char.ToUpperInvariant(x) == char.ToUpperInvariant(y);
    }

    int IEqualityComparer<char>.GetHashCode(char obj)
    {
        return char.ToUpperInvariant(obj).GetHashCode();
    }
}