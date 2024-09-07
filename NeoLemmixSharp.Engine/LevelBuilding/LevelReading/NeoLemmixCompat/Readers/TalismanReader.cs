using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class TalismanReader : INeoLemmixDataReader
{
    private readonly CaseInvariantCharEqualityComparer _charEqualityComparer;
    private TalismanData? _currentTalismanData;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$TALISMAN";

    public List<TalismanData> TalismanData { get; } = new();

    public TalismanReader(CaseInvariantCharEqualityComparer charEqualityComparer)
    {
        _charEqualityComparer = charEqualityComparer;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentTalismanData = new TalismanData();
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        if (firstToken.EndsWith("_LIMIT"))
        {
            ParseLimitTokens(firstToken, secondToken);

            return false;
        }

        var currentTalismanData = _currentTalismanData!;

        switch (firstToken)
        {
            case "TITLE":
                currentTalismanData.Title = line.TrimAfterIndex(secondTokenIndex).ToString();
                break;

            case "ID":
                currentTalismanData.Id = int.Parse(secondToken);
                break;

            case "COLOR":
                currentTalismanData.Color = GetTalismanColor(secondToken);
                break;

            case "SAVE_REQUIREMENT":
                currentTalismanData.SaveRequirement = int.Parse(secondToken);
                break;

            case "USE_ONLY_SKILL":
                if (!NxlvReadingHelpers.GetSkillByName(secondToken, _charEqualityComparer, out var onlySkill))
                {
                    NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
                    break;
                }

                foreach (var item in LemmingSkill.AllItems)
                {
                    currentTalismanData.SkillLimits.Add(item, 0);
                }

                currentTalismanData.SkillLimits.Remove(onlySkill);
                break;

            case "$END":
                TalismanData.Add(currentTalismanData);
                _currentTalismanData = null;
                FinishedReading = true;
                break;

            default:
                NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
                break;
        }

        return false;
    }

    private TalismanColor GetTalismanColor(ReadOnlySpan<char> secondToken)
    {
        // Need to use case invariant comparer since different versions
        // of the NeoLemmix engine saved the talisman colors with different
        // character casing - "GOLD", "Gold", etc.
        if (secondToken.SequenceEqual("BRONZE", _charEqualityComparer))
            return TalismanColor.Bronze;

        if (secondToken.SequenceEqual("SILVER", _charEqualityComparer))
            return TalismanColor.Silver;

        if (secondToken.SequenceEqual("GOLD", _charEqualityComparer))
            return TalismanColor.Gold;

        return NxlvReadingHelpers.ThrowUnknownTokenException<TalismanColor>(IdentifierToken, "COLOR", secondToken);
    }

    private void ParseLimitTokens(ReadOnlySpan<char> firstToken, ReadOnlySpan<char> secondToken)
    {
        var currentTalismanData = _currentTalismanData!;

        if (firstToken is "TIME_LIMIT")
        {
            currentTalismanData.TimeLimitInSeconds = int.Parse(secondToken);
            return;
        }

        if (firstToken is "SKILL_LIMIT")
        {
            currentTalismanData.AllSkillLimit = int.Parse(secondToken);
            return;
        }

        if (NxlvReadingHelpers.GetSkillByName(firstToken[..^6], _charEqualityComparer, out var skill))
        {
            currentTalismanData.SkillLimits.Add(skill, int.Parse(secondToken));
            return;
        }

        NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, secondToken);
    }
}