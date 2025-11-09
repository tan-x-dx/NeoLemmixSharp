using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level.Objectives;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

internal sealed class SkillSetReader : NeoLemmixDataReader
{
    private readonly HashSet<string> _seenSkills = new(10);
    private readonly UniqueStringSet _uniqueStringSet;

    public List<SkillSetData> SkillSetData { get; } = new();

    public SkillSetReader(UniqueStringSet uniqueStringSet)
        : base("$SKILLSET")
    {
        _uniqueStringSet = uniqueStringSet;
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
        return false;
    }

    public override bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

        if (Helpers.StringSpansMatch(firstToken, "$END"))
        {
            FinishedReading = true;
            return false;
        }

        if (!LemmingSkillConstants.TryGetLemmingSkillIdFromName(firstToken, out var skillId))
            throw new FileReadingException($"Unknown token: {firstToken}");

        var skillName = _uniqueStringSet.GetUniqueStringInstance(firstToken);

        if (!_seenSkills.Add(skillName))
            throw new FileReadingException($"Skill recorded multiple times! {skillName}");

        var amount = Helpers.StringSpansMatch(secondToken, "INFINITE")
            ? EngineConstants.InfiniteSkillCount
            : int.Parse(secondToken);

        if ((uint)amount > EngineConstants.InfiniteSkillCount)
            throw new FileReadingException($"Invalid skill count value! {amount}");

        if (skillId == LemmingSkillConstants.ClonerSkillId && amount == EngineConstants.InfiniteSkillCount)
        {
            amount = EngineConstants.MaxFiniteSkillCount;
        }

        var skillSetDatum = new SkillSetData(skillId, EngineConstants.ClassicTribeId, amount);

        SkillSetData.Add(skillSetDatum);
        return false;
    }
}
