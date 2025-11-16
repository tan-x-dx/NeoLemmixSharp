using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Objectives;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class LevelObjectiveDataSectionReader : LevelDataSectionReader
{
    private readonly FileReaderStringIdLookup _stringIdLookup;

    public LevelObjectiveDataSectionReader(
        FileReaderStringIdLookup stringIdLookup)
        : base(LevelFileSectionIdentifier.LevelObjectivesDataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawLevelFileDataReader reader, LevelData levelData, int numberOfItemsInSection)
    {
        FileReadingException.ReaderAssert(numberOfItemsInSection == 1, "Expected ONE level objective item!");

        int objectiveNameId = reader.Read16BitUnsignedInteger();

        var skillSetData = ReadSkillSetData(reader);
        var objectiveCriteria = ReadObjectiveCriteria(reader);
        var objectiveModifiers = ReadObjectiveModifiers(reader);
        var talismanData = ReadTalismanData(reader);

        var objectiveData = new LevelObjectiveData
        {
            ObjectiveName = _stringIdLookup[objectiveNameId],

            SkillSetData = skillSetData,
            ObjectiveCriteria = objectiveCriteria,
            ObjectiveModifiers = objectiveModifiers,
            TalismanData = talismanData
        };

        levelData.SetObjectiveData(objectiveData);
    }

    private static SkillSetData[] ReadSkillSetData(RawLevelFileDataReader reader)
    {
        int numberOfSkillSetData = reader.Read16BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<SkillSetData>(numberOfSkillSetData);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadSkillSetDatum(reader);
        }

        return result;
    }

    private static SkillSetData ReadSkillSetDatum(RawLevelFileDataReader reader)
    {
        int skillId = reader.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert(LemmingSkillConstants.IsValidLemmingSkillId(skillId), "Invalid skill id");

        int tribeId = reader.Read8BitUnsignedInteger();
        tribeId--; // Need to offset by 1
        FileReadingException.ReaderAssert(tribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe id");

        int initialQuantity = reader.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert(initialQuantity <= EngineConstants.InfiniteSkillCount, "Invalid skill count quantity");

        return new SkillSetData(skillId, tribeId, initialQuantity);
    }

    private static ObjectiveCriterionData[] ReadObjectiveCriteria(RawLevelFileDataReader reader)
    {
        int numberOfObjectiveCriteria = reader.Read8BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<ObjectiveCriterionData>(numberOfObjectiveCriteria);

        for (var i = 0; i < result.Length; i++)
        {
            var newObjectiveCriterion = ReadObjectiveCriterion(reader);

            for (var j = 0; j < i; j++)
            {
                var existingObjectiveCriterion = result[j];

                FileReadingException.ReaderAssert(!existingObjectiveCriterion.MatchesBaseCriterionData(newObjectiveCriterion), "Redundant objective criteria!");
            }

            result[i] = newObjectiveCriterion;
        }

        return result;
    }

    private static ObjectiveCriterionData ReadObjectiveCriterion(RawLevelFileDataReader reader)
    {
        uint rawObjectiveCriterionId = reader.Read8BitUnsignedInteger();
        var objectiveCriterionType = (ObjectiveCriterionType)rawObjectiveCriterionId;

        return objectiveCriterionType switch
        {
            ObjectiveCriterionType.SaveLemmings => CreateSaveLemmingsCriterion(),
            ObjectiveCriterionType.TimeLimit => CreateTimeLimitCriterion(),
            ObjectiveCriterionType.KillAllZombies => new KillAllZombiesCriterionData(),

            _ => Helpers.ThrowUnknownEnumValueException<ObjectiveCriterionType, ObjectiveCriterionData>(objectiveCriterionType),
        };

        SaveLemmingsCriterionData CreateSaveLemmingsCriterion()
        {
            int saveRequirement = reader.Read16BitUnsignedInteger();
            int tribeId = reader.Read8BitUnsignedInteger();
            tribeId--; // Need to offset by 1

            FileReadingException.ReaderAssert(tribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe id");

            return new SaveLemmingsCriterionData
            {
                SaveRequirement = saveRequirement,
                TribeId = tribeId
            };
        }

        TimeLimitCriterionData CreateTimeLimitCriterion()
        {
            uint timeLimitInSeconds = reader.Read16BitUnsignedInteger();
            FileReadingException.ReaderAssert(timeLimitInSeconds <= EngineConstants.MaxTimeLimitInSeconds, "Invalid time limit");

            return new TimeLimitCriterionData
            {
                TimeLimitInSeconds = timeLimitInSeconds
            };
        }
    }

    private static ObjectiveModifierData[] ReadObjectiveModifiers(RawLevelFileDataReader reader)
    {
        int numberOfObjectiveModifiers = reader.Read8BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<ObjectiveModifierData>(numberOfObjectiveModifiers);

        for (var i = 0; i < result.Length; i++)
        {
            var newObjectiveModifier = ReadObjectiveModifier(reader);

            for (var j = 0; j < i; j++)
            {
                var existingObjectiveModifier = result[j];

                FileReadingException.ReaderAssert(!existingObjectiveModifier.MatchesBaseModifierData(newObjectiveModifier), "Redundant objective modifier!");
            }

            result[i] = newObjectiveModifier;
        }

        return result;
    }

    private static ObjectiveModifierData ReadObjectiveModifier(RawLevelFileDataReader reader)
    {
        uint rawObjectiveModifierId = reader.Read8BitUnsignedInteger();
        var objectiveModifierType = (ObjectiveModifierType)rawObjectiveModifierId;

        return objectiveModifierType switch
        {
            ObjectiveModifierType.LimitSpecificSkillAssignments => CreateLimitSpecificSkillAssignmentsModifier(),
            ObjectiveModifierType.LimitTotalSkillAssignments => CreateLimitTotalSkillAssignmentsModifier(),

            _ => Helpers.ThrowUnknownEnumValueException<ObjectiveModifierType, ObjectiveModifierData>(objectiveModifierType),
        };

        LimitSpecificSkillAssignmentsModifierData CreateLimitSpecificSkillAssignmentsModifier()
        {
            int skillId = reader.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(LemmingSkillConstants.IsValidLemmingSkillId(skillId), "Invalid skill id");

            int tribeId = reader.Read8BitUnsignedInteger();
            tribeId--; // Need to offset by 1
            FileReadingException.ReaderAssert(tribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe id");

            int maxSkillAssignments = reader.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(maxSkillAssignments <= EngineConstants.MaxFiniteSkillCount, "Invalid skill limit quantity");

            return new LimitSpecificSkillAssignmentsModifierData
            {
                SkillId = skillId,
                TribeId = tribeId,
                MaxSkillAssignments = maxSkillAssignments,
            };
        }

        LimitTotalSkillAssignmentsModifierData CreateLimitTotalSkillAssignmentsModifier()
        {
            int maxTotalSkillAssignments = reader.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(maxTotalSkillAssignments <= EngineConstants.MaxFiniteSkillCount, "Invalid skill limit quantity");

            return new LimitTotalSkillAssignmentsModifierData
            {
                MaxTotalSkillAssignments = maxTotalSkillAssignments
            };
        }
    }

    private TalismanData[] ReadTalismanData(RawLevelFileDataReader reader)
    {
        int numberOfTalismanData = reader.Read8BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<TalismanData>(numberOfTalismanData);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadTalismanDatum(reader);
        }

        return result;
    }

    private TalismanData ReadTalismanDatum(RawLevelFileDataReader reader)
    {
        int talismanId = reader.Read8BitUnsignedInteger();
        int talismanNameId = reader.Read16BitUnsignedInteger();

        uint rawRankId = reader.Read8BitUnsignedInteger();
        var rank = TalismanRankHelpers.GetEnumValue(rawRankId);

        var overrideObjectiveCriteria = ReadObjectiveCriteria(reader);
        var overrideObjectiveModifiers = ReadObjectiveModifiers(reader);

        return new TalismanData
        {
            TalismanId = talismanId,
            TalismanName = _stringIdLookup[talismanNameId],
            Rank = rank,

            OverrideObjectiveCriteria = overrideObjectiveCriteria,
            OverrideObjectiveModifiers = overrideObjectiveModifiers
        };
    }
}
