using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class SkillSetReader : NeoLemmixDataReader
{
    private readonly LemmingSkillSet _seenSkills = LemmingSkill.CreateEmptySimpleSet();

    public List<SkillSetData> SkillSetData { get; } = new();

    public SkillSetReader()
        : base("$SKILLSET")
    {
    }

    public override void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
    }

    public override bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

        if (TokensMatch(firstToken, "$END"))
        {
            FinishedReading = true;
            return false;
        }

        if (!NxlvReadingHelpers.TryGetSkillByName(firstToken, Helpers.CaseInvariantCharEqualityComparer, out var skill))
            throw new InvalidOperationException($"Unknown token: {firstToken}");

        if (!_seenSkills.Add(skill))
            throw new InvalidOperationException($"Skill recorded multiple times! {skill.LemmingSkillName}");

        var amount = TokensMatch(secondToken, "INFINITE")
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