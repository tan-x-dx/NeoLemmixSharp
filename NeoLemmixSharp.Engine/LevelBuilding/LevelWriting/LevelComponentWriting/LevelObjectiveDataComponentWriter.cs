using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelWriting.LevelComponentWriting;

public sealed class LevelObjectiveDataComponentWriter : ILevelDataWriter
{
    private readonly Dictionary<string, ushort> _stringIdLookup;

    public LevelObjectiveDataComponentWriter(Dictionary<string, ushort> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.LevelObjectivesDataSectionIdentifier;

    public ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)(1 + levelData.LevelObjectives.Count);
    }

    public void WriteSection(
        BinaryWriter writer,
        LevelData levelData)
    {
        foreach (var levelObjective in levelData.LevelObjectives)
        {
            WriteLevelObjective(writer, levelObjective);
        }
    }

    private void WriteLevelObjective(
        BinaryWriter writer,
        LevelObjective levelObjective)
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
        return (ushort)(LevelReadWriteHelpers.NumberOfBytesForMainLevelObjectiveData + LevelReadWriteHelpers.NumberOfBytesPerSkillSetDatum * levelObjective.SkillSetData.Length + LevelReadWriteHelpers.NumberOfBytesPerRequirementsDatum * levelObjective.Requirements.Length);
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