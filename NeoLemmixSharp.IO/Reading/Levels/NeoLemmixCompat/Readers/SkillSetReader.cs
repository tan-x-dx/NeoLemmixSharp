using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

public sealed class SkillSetReader : NeoLemmixDataReader
{
  //  private readonly LemmingSkillSet _seenSkills = LemmingSkill.CreateBitArraySet();

    public List<SkillSetData> SkillSetData { get; } = new();

    public SkillSetReader()
        : base("$SKILLSET")
    {
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
        return false;
    }

    public override bool ReadNextLine(ReadOnlySpan<char> line)
    {
     /*   NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

        if (TokensMatch(firstToken, "$END"))
        {
            FinishedReading = true;
            return false;
        }

        if (!NxlvReadingHelpers.TryGetSkillByName(firstToken, out var skill))
            throw new FileReadingException($"Unknown token: {firstToken}");

        if (!_seenSkills.Add(skill))
            throw new FileReadingException($"Skill recorded multiple times! {skill.LemmingSkillName}");

        var amount = TokensMatch(secondToken, "INFINITE")
            ? EngineConstants.InfiniteSkillCount
            : int.Parse(secondToken);

        if ((uint)amount >= EngineConstants.InfiniteSkillCount)
            throw new FileReadingException($"Invalid skill count value! {amount}");

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

        SkillSetData.Add(skillSetDatum);*/
        return false;
    }
}