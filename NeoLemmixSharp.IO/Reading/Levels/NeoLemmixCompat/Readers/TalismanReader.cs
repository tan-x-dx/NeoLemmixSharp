using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level.Objectives;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

internal sealed class TalismanReader : NeoLemmixDataReader
{
    private readonly UniqueStringSet _uniqueStringSet;
    private Data.TalismanData? _currentTalismanData;

    public List<Data.TalismanData> TalismanData { get; } = new();

    public TalismanReader(
        UniqueStringSet uniqueStringSet)
        : base("$TALISMAN")
    {
        _uniqueStringSet = uniqueStringSet;

        SetNumberOfTokens(6);

        RegisterTokenAction("TITLE", SetTitle);
        RegisterTokenAction("ID", SetId);
        RegisterTokenAction("COLOR", SetColor);
        RegisterTokenAction("SAVE_REQUIREMENT", SetSaveRequirement);
        RegisterTokenAction("USE_ONLY_SKILL", SetUseOnlySkill);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        _currentTalismanData = new Data.TalismanData();
        FinishedReading = false;
        return false;
    }

    public override bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        if (firstToken.EndsWith("_LIMIT"))
        {
            ParseLimitTokens(firstToken, secondToken);

            return false;
        }

        return ProcessLineTokens(line);
    }

    private void SetTitle(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTalismanData!.Title = _uniqueStringSet.GetUniqueStringInstance(line[secondTokenIndex..]);
    }

    private void SetId(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTalismanData!.Id = int.Parse(secondToken);
    }

    private void SetColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTalismanData!.Color = GetTalismanColor(secondToken);
    }

    private void SetSaveRequirement(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTalismanData!.SaveRequirement = int.Parse(secondToken);
    }

    private void SetUseOnlySkill(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (!LemmingSkillConstants.TryGetLemmingSkillIdFromName(secondToken, out var onlySkill))
        {
            NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, "USE_ONLY_SKILL", line);
            return;
        }

        /*    foreach (var item in LemmingSkill.AllItems)
            {
                _currentTalismanData!.SkillLimits.Add(item, 0);
            }

            _currentTalismanData!.SkillLimits.Remove(onlySkill);*/
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        throw new NotImplementedException();
    }

    private TalismanRank GetTalismanColor(ReadOnlySpan<char> secondToken)
    {
        if (TokensMatch(secondToken, "BRONZE"))
            return TalismanRank.Bronze;

        if (TokensMatch(secondToken, "SILVER"))
            return TalismanRank.Silver;

        if (TokensMatch(secondToken, "GOLD"))
            return TalismanRank.Gold;

        return NxlvReadingHelpers.ThrowUnknownTokenException<TalismanRank>(IdentifierToken, "COLOR", secondToken);
    }

    private void ParseLimitTokens(ReadOnlySpan<char> firstToken, ReadOnlySpan<char> secondToken)
    {
        var currentTalismanData = _currentTalismanData!;

        if (TokensMatch(firstToken, "TIME_LIMIT"))
        {
            currentTalismanData.TimeLimitInSeconds = int.Parse(secondToken);
            return;
        }

        if (TokensMatch(firstToken, "SKILL_LIMIT"))
        {
            currentTalismanData.AllSkillLimit = int.Parse(secondToken);
            return;
        }

        if (LemmingSkillConstants.TryGetLemmingSkillIdFromName(firstToken[..^6], out var skill))
        {
            //     currentTalismanData.SkillLimits.Add(skill, int.Parse(secondToken));
            return;
        }

        NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, secondToken);
    }
}
