using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Objectives.Requirements;
using NeoLemmixSharp.Engine.LevelIo.Data.Level;

namespace NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections.Version1_0_0_0;

public sealed class LevelObjectiveDataSectionWriter : LevelDataSectionWriter
{
    public override bool IsNecessary => true;

    private readonly Dictionary<string, ushort> _stringIdLookup;

    public LevelObjectiveDataSectionWriter(Dictionary<string, ushort> stringIdLookup)
        : base(LevelFileSectionIdentifier.LevelObjectivesDataSection)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return (ushort)levelData.LevelObjectives.Count;
    }

    public override void WriteSection(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        foreach (var levelObjective in levelData.LevelObjectives)
        {
            WriteLevelObjective(writer, levelObjective);
        }
    }

    private void WriteLevelObjective(
        RawLevelFileDataWriter writer,
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

    private static ushort GetNumberOfBytesForLevelObjective(
        LevelObjective levelObjective)
    {
        return (ushort)(LevelReadWriteHelpers.NumberOfBytesForMainLevelObjectiveData +
                        LevelReadWriteHelpers.NumberOfBytesPerSkillSetDatum * levelObjective.SkillSetData.Length +
                        LevelReadWriteHelpers.NumberOfBytesPerRequirementsDatum * levelObjective.Requirements.Length);
    }

    private static void WriteSkillSetDatum(
        RawLevelFileDataWriter writer,
        SkillSetData skillSetData)
    {
        writer.Write((byte)skillSetData.Skill.Id);
        writer.Write((byte)skillSetData.NumberOfSkills);
        writer.Write((byte)skillSetData.TeamId);
    }

    private static void WriteRequirements(
        RawLevelFileDataWriter writer,
        IObjectiveRequirement objectiveRequirement)
    {
        // TODO
    }
}