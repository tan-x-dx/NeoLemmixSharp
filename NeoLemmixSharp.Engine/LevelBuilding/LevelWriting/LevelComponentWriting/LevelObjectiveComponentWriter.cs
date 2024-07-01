using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public readonly ref struct LevelObjectiveComponentWriter
{
    private const int NumberOfBytesPerSkillSetDatum = 3;
    private const int NumberOfBytesPerRequirementsDatum = 4;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public LevelObjectiveComponentWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    private static ReadOnlySpan<byte> GetSectionIdentifier()
    {
        ReadOnlySpan<byte> sectionIdentifier = [0xBE, 0xF4];
        return sectionIdentifier;
    }

    private static ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)(1 + levelData.SecondaryLevelObjectives.Count);
    }

    public void WriteSection(BinaryWriter writer, LevelData levelData)
    {
        writer.Write(GetSectionIdentifier());
        writer.Write(CalculateNumberOfItemsInSection(levelData));

        WriteLevelObjective(writer, levelData.PrimaryLevelObjective!);
        foreach (var secondaryLevelObjective in levelData.SecondaryLevelObjectives)
        {
            WriteLevelObjective(writer, secondaryLevelObjective);
        }
    }

    private void WriteLevelObjective(BinaryWriter writer, LevelObjective levelObjective)
    {
        writer.Write(GetNumberOfBytesForLevelObjective(levelObjective));

        writer.Write((byte)levelObjective.LevelObjectiveId);
        var titleStringId = _stringIdLookup.GetValueOrDefault(levelObjective.LevelObjectiveTitle);
        writer.Write(titleStringId);

        writer.Write((ushort)levelObjective.SkillSetData.Length);
        foreach (var skillSetDatum in levelObjective.SkillSetData)
        {
            WriteSkillSetDatum(writer, skillSetDatum);
        }

        writer.Write((ushort)levelObjective.Requirements.Length);
        foreach (var levelObjectiveRequirement in levelObjective.Requirements)
        {
            WriteRequirements(writer, levelObjectiveRequirement);
        }
    }

    private static ushort GetNumberOfBytesForLevelObjective(LevelObjective levelObjective)
    {
        return (ushort)(7 +
                        (NumberOfBytesPerSkillSetDatum * levelObjective.SkillSetData.Length) +
                        (NumberOfBytesPerRequirementsDatum * levelObjective.Requirements.Length));
    }

    private static void WriteSkillSetDatum(BinaryWriter writer, SkillSetData skillSetData)
    {
        writer.Write((byte)skillSetData.Skill.Id);
        writer.Write((byte)skillSetData.NumberOfSkills);
        writer.Write((byte)skillSetData.TeamId);
    }

    private static void WriteRequirements(BinaryWriter writer, IObjectiveRequirement objectiveRequirement)
    {
        // TODO
    }
}