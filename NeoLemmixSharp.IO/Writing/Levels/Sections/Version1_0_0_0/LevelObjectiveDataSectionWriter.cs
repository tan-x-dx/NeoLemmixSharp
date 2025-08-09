using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Objectives;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Levels.Sections.Version1_0_0_0;

internal sealed class LevelObjectiveDataSectionWriter : LevelDataSectionWriter
{
    private readonly FileWriterStringIdLookup _stringIdLookup;

    public LevelObjectiveDataSectionWriter(FileWriterStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.LevelObjectivesDataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(LevelData levelData)
    {
        return 1;
    }

    public override void WriteSection(
        RawLevelFileDataWriter writer,
        LevelData levelData)
    {
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(levelData.LevelObjective.ObjectiveName));

        WriteSkillSetData(writer, levelData.LevelObjective.SkillSetData);
        WriteObjectiveCriteria(writer, levelData.LevelObjective.ObjectiveCriteria);
        WriteObjectiveModifiers(writer, levelData.LevelObjective.ObjectiveModifiers);
        WriteTalismanData(writer, levelData.LevelObjective.TalismanData);
    }

    private static void WriteSkillSetData(RawLevelFileDataWriter writer, SkillSetData[] skillSetData)
    {
        writer.Write16BitUnsignedInteger((ushort)skillSetData.Length);

        foreach (var skillSetDatum in skillSetData)
        {
            WriteSkillSetDatum(writer, skillSetDatum);
        }
    }

    private static void WriteSkillSetDatum(RawLevelFileDataWriter writer, SkillSetData skillSetDatum)
    {
        FileWritingException.WriterAssert(LemmingSkillConstants.IsValidLemmingSkillId(skillSetDatum.SkillId), "Invalid skill id");
        FileWritingException.WriterAssert(skillSetDatum.TribeId >= -1, "Invalid tribe id");
        FileWritingException.WriterAssert(skillSetDatum.TribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe id");
        FileWritingException.WriterAssert(skillSetDatum.InitialQuantity >= 0, "Invalid skill limit quantity");
        FileWritingException.WriterAssert(skillSetDatum.InitialQuantity <= EngineConstants.InfiniteSkillCount, "Invalid skill limit quantity");

        writer.Write8BitUnsignedInteger((byte)skillSetDatum.SkillId);
        writer.Write8BitUnsignedInteger((byte)(skillSetDatum.TribeId + 1)); // Need to offset by 1
        writer.Write8BitUnsignedInteger((byte)skillSetDatum.InitialQuantity);
    }

    private static void WriteObjectiveCriteria(RawLevelFileDataWriter writer, ObjectiveCriterionData[] objectiveCriteria)
    {
        writer.Write8BitUnsignedInteger((byte)objectiveCriteria.Length);

        foreach (var objectiveCriterion in objectiveCriteria)
        {
            WriteObjectiveCriterion(writer, objectiveCriterion);
        }
    }

    private static void WriteObjectiveCriterion(RawLevelFileDataWriter writer, ObjectiveCriterionData objectiveCriterion)
    {
        writer.Write8BitUnsignedInteger((byte)objectiveCriterion.Type);

        switch (objectiveCriterion.Type)
        {
            case ObjectiveCriterionType.SaveLemmings:
                WriteSaveLemmingsCriterion();
                return;

            case ObjectiveCriterionType.TimeLimit:
                WriteTimeLimitCriterion();
                return;

            case ObjectiveCriterionType.KillAllZombies:
                // No extra data necessary
                return;

            default:
                Helpers.ThrowUnknownEnumValueException<ObjectiveCriterionType, ObjectiveCriterionType>(objectiveCriterion.Type);
                return;
        }

        void WriteSaveLemmingsCriterion()
        {
            var saveLemmingsCriterion = (SaveLemmingsCriterionData)objectiveCriterion;

            FileWritingException.WriterAssert(saveLemmingsCriterion.TribeId >= -1, "Invalid tribe id");
            FileWritingException.WriterAssert(saveLemmingsCriterion.TribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe id");

            writer.Write16BitUnsignedInteger((ushort)saveLemmingsCriterion.SaveRequirement);
            writer.Write8BitUnsignedInteger((byte)(saveLemmingsCriterion.TribeId + 1));// Need to offset by 1
        }

        void WriteTimeLimitCriterion()
        {
            var timeLimitCriterion = (TimeLimitCriterionData)objectiveCriterion;

            FileWritingException.WriterAssert(timeLimitCriterion.TimeLimitInSeconds > 0, "Invalid time limit");
            FileWritingException.WriterAssert(timeLimitCriterion.TimeLimitInSeconds <= EngineConstants.MaxTimeLimitInSeconds, "Invalid time limit");

            writer.Write16BitUnsignedInteger((ushort)timeLimitCriterion.TimeLimitInSeconds);
        }
    }

    private static void WriteObjectiveModifiers(RawLevelFileDataWriter writer, ObjectiveModifierData[] objectiveModifiers)
    {
        writer.Write8BitUnsignedInteger((byte)objectiveModifiers.Length);

        foreach (var objectiveModifier in objectiveModifiers)
        {
            WriteObjectiveModifier(writer, objectiveModifier);
        }
    }

    private static void WriteObjectiveModifier(RawLevelFileDataWriter writer, ObjectiveModifierData objectiveModifier)
    {
        writer.Write8BitUnsignedInteger((byte)objectiveModifier.Type);

        switch (objectiveModifier.Type)
        {
            case ObjectiveModifierType.LimitSpecificSkillAssignments:
                WriteLimitSpecificSkillModifier();
                return;

            case ObjectiveModifierType.LimitTotalSkillAssignments:
                WriteTotalSkillModifier();
                return;

            default:
                Helpers.ThrowUnknownEnumValueException<ObjectiveModifierType, ObjectiveModifierType>(objectiveModifier.Type);
                return;
        }

        void WriteLimitSpecificSkillModifier()
        {
            var limitSpecificSkillModifier = (LimitSpecificSkillAssignmentsModifierData)objectiveModifier;

            FileWritingException.WriterAssert(LemmingSkillConstants.IsValidLemmingSkillId(limitSpecificSkillModifier.SkillId), "Invalid skill id");
            FileWritingException.WriterAssert(limitSpecificSkillModifier.TribeId >= -1, "Invalid tribe id");
            FileWritingException.WriterAssert(limitSpecificSkillModifier.TribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe id");
            FileWritingException.WriterAssert(limitSpecificSkillModifier.MaxSkillAssignments >= 0, "Invalid skill limit quantity");
            FileWritingException.WriterAssert(limitSpecificSkillModifier.MaxSkillAssignments <= EngineConstants.MaxFiniteSkillCount, "Invalid skill limit quantity");

            writer.Write8BitUnsignedInteger((byte)limitSpecificSkillModifier.SkillId);
            writer.Write8BitUnsignedInteger((byte)(limitSpecificSkillModifier.TribeId + 1)); // Need to offset by 1
            writer.Write8BitUnsignedInteger((byte)limitSpecificSkillModifier.MaxSkillAssignments);
        }

        void WriteTotalSkillModifier()
        {
            var limitTotalSkillModifier = (LimitTotalSkillAssignmentsModifierData)objectiveModifier;

            FileWritingException.WriterAssert(limitTotalSkillModifier.MaxTotalSkillAssignments >= 0, "Invalid skill limit quantity");
            FileWritingException.WriterAssert(limitTotalSkillModifier.MaxTotalSkillAssignments <= EngineConstants.MaxFiniteSkillCount, "Invalid skill limit quantity");

            writer.Write16BitUnsignedInteger((ushort)limitTotalSkillModifier.MaxTotalSkillAssignments);
        }
    }

    private void WriteTalismanData(RawLevelFileDataWriter writer, TalismanData[] talismanData)
    {
        writer.Write8BitUnsignedInteger((byte)talismanData.Length);

        foreach (var talisman in talismanData)
        {
            WriteTalismanDatum(writer, talisman);
        }
    }

    private void WriteTalismanDatum(RawLevelFileDataWriter writer, TalismanData talisman)
    {
        writer.Write8BitUnsignedInteger((byte)talisman.TalismanId);
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(talisman.TalismanName));
        writer.Write8BitUnsignedInteger((byte)talisman.Rank);

        WriteObjectiveCriteria(writer, talisman.OverrideObjectiveCriteria);
        WriteObjectiveModifiers(writer, talisman.OverrideObjectiveModifiers);
    }
}
