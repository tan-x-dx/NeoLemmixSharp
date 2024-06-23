﻿using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class SkillSetReader : INeoLemmixDataReader
{
    private readonly CaseInvariantCharEqualityComparer _charEqualityComparer;
    private readonly SimpleSet<LemmingSkill> _seenSkills = ExtendedEnumTypeComparer<LemmingSkill>.CreateSimpleSet();

    public List<SkillSetDatum> SkillSetData { get; } = new();

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$SKILLSET";

    public SkillSetReader(CaseInvariantCharEqualityComparer charEqualityComparer)
    {
        _charEqualityComparer = charEqualityComparer;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        ReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

        if (firstToken is "$END")
        {
            FinishedReading = true;
            return false;
        }

        if (!ReadingHelpers.GetSkillByName(firstToken, _charEqualityComparer, out var skill))
            throw new InvalidOperationException($"Unknown token: {firstToken}");

        if (!_seenSkills.Add(skill))
            throw new InvalidOperationException($"Skill recorded multiple times! {skill.LemmingSkillName}");

        var amount = secondToken is "INFINITE"
            ? LevelConstants.InfiniteSkillCount
            : int.Parse(secondToken);

        if (amount < 0 ||
            amount > LevelConstants.InfiniteSkillCount)
            throw new InvalidOperationException($"Invalid skill count value! {amount}");

        var skillSetDatum = new SkillSetDatum
        {
            Skill = skill,
            NumberOfSkills = amount,
            TeamId = LevelConstants.ClassicTeamId
        };

        SkillSetData.Add(skillSetDatum);
        return false;
    }
}