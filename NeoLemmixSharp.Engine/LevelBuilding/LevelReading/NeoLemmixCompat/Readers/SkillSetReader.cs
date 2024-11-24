using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class SkillSetReader : INeoLemmixDataReader
{
    private readonly IEqualityComparer<char> _charEqualityComparer;
    private readonly LemmingSkillSet _seenSkills = LemmingSkill.CreateEmptySimpleSet();

    public List<SkillSetData> SkillSetData { get; } = new();

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$SKILLSET";

    public SkillSetReader(IEqualityComparer<char> charEqualityComparer)
    {
        _charEqualityComparer = charEqualityComparer;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

        if (firstToken is "$END")
        {
            FinishedReading = true;
            return false;
        }

        if (!NxlvReadingHelpers.GetSkillByName(firstToken, _charEqualityComparer, out var skill))
            throw new InvalidOperationException($"Unknown token: {firstToken}");

        if (!_seenSkills.Add(skill))
            throw new InvalidOperationException($"Skill recorded multiple times! {skill.LemmingSkillName}");

        var amount = secondToken is "INFINITE"
            ? EngineConstants.InfiniteSkillCount
            : int.Parse(secondToken);

        if (amount < 0 ||
            amount > EngineConstants.InfiniteSkillCount)
            throw new InvalidOperationException($"Invalid skill count value! {amount}");

        if (skill == ClonerSkill.Instance && amount == EngineConstants.InfiniteSkillCount)
        {
            amount = EngineConstants.InfiniteSkillCount - 1;
        }

        var skillSetDatum = new SkillSetData
        {
            Skill = skill,
            NumberOfSkills = amount,
            TeamId = EngineConstants.ClassicTeamId
        };

        SkillSetData.Add(skillSetDatum);
        return false;
    }
}