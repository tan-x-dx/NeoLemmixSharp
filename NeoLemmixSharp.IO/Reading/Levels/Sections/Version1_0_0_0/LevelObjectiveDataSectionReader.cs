using NeoLemmixSharp.Common;
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

    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData, int numberOfItemsInSection)
    {
        FileReadingException.ReaderAssert(numberOfItemsInSection == 1, "Expected ONE level objective item!");

        int objectiveNameId = rawFileData.Read16BitUnsignedInteger();

        var skillSetData = ReadSkillSetData(rawFileData);
        var objectiveCriteria = ReadObjectiveCriteria(rawFileData);
        var objectiveModifiers = ReadObjectiveModifiers(rawFileData);
        var talismanData = ReadTalismanData(rawFileData);

        var objectiveData = new ObjectiveData
        {
            ObjectiveName = _stringIdLookup[objectiveNameId],

            SkillSetData = skillSetData,
            ObjectiveCriteria = objectiveCriteria,
            ObjectiveModifiers = objectiveModifiers,
            TalismanData = talismanData
        };

        levelData.SetObjectiveData(objectiveData);
    }

    private static SkillSetData[] ReadSkillSetData(RawLevelFileDataReader rawFileData)
    {
        int numberOfSkillSetData = rawFileData.Read16BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<SkillSetData>(numberOfSkillSetData);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadSkillSetDatum(rawFileData);
        }

        return result;
    }

    private static SkillSetData ReadSkillSetDatum(RawLevelFileDataReader rawFileData)
    {
        int skillId = rawFileData.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert(LemmingSkillConstants.IsValidLemmingSkillId(skillId), "Invalid skill id");

        int initialQuantity = rawFileData.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert(initialQuantity <= EngineConstants.InfiniteSkillCount, "Invalid skill count quantity");

        int tribeId = rawFileData.Read8BitUnsignedInteger();
        tribeId--; // Need to offset by 1
        FileReadingException.ReaderAssert(tribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe id");

        return new SkillSetData(skillId, initialQuantity, tribeId);
    }

    private static ObjectiveCriterion[] ReadObjectiveCriteria(RawLevelFileDataReader rawFileData)
    {
        int numberOfObjectiveCriteria = rawFileData.Read8BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<ObjectiveCriterion>(numberOfObjectiveCriteria);

        for (var i = 0; i < result.Length; i++)
        {
            var newObjectiveCriterion = ReadObjectiveCriterion(rawFileData);

            for (var j = 0; j < i; j++)
            {
                var existingObjectiveCriterion = result[j];

                FileReadingException.ReaderAssert(!existingObjectiveCriterion.MatchesBaseCriterionData(newObjectiveCriterion), "Redundant objective criteria!");
            }

            result[i] = newObjectiveCriterion;
        }

        return result;
    }

    private static ObjectiveCriterion ReadObjectiveCriterion(RawLevelFileDataReader rawFileData)
    {
        uint rawObjectiveCriterionId = rawFileData.Read8BitUnsignedInteger();
        var objectiveCriterionType = (ObjectiveCriterionType)rawObjectiveCriterionId;

        return objectiveCriterionType switch
        {
            ObjectiveCriterionType.SaveLemmings => CreateSaveLemmingsCriterion(),
            ObjectiveCriterionType.TimeLimit => CreateTimeLimitCriterion(),
            ObjectiveCriterionType.KillAllZombies => new KillAllZombiesCriterion(),

            _ => Helpers.ThrowUnknownEnumValueException<ObjectiveCriterionType, ObjectiveCriterion>(objectiveCriterionType),
        };

        SaveLemmingsCriterion CreateSaveLemmingsCriterion()
        {
            int saveRequirement = rawFileData.Read16BitUnsignedInteger();
            int tribeId = rawFileData.Read8BitUnsignedInteger();
            tribeId--; // Need to offset by 1

            FileReadingException.ReaderAssert(tribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe id");

            return new SaveLemmingsCriterion
            {
                SaveRequirement = saveRequirement,
                TribeId = tribeId
            };
        }

        TimeLimitCriterion CreateTimeLimitCriterion()
        {
            int timeLimitInSeconds = rawFileData.Read16BitUnsignedInteger();
            FileReadingException.ReaderAssert(timeLimitInSeconds < EngineConstants.MaxTimeLimitInSeconds, "Invalid time limit");

            return new TimeLimitCriterion
            {
                TimeLimitInSeconds = timeLimitInSeconds
            };
        }
    }

    private static ObjectiveModifier[] ReadObjectiveModifiers(RawLevelFileDataReader rawFileData)
    {
        int numberOfObjectiveModifiers = rawFileData.Read8BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<ObjectiveModifier>(numberOfObjectiveModifiers);

        for (var i = 0; i < result.Length; i++)
        {
            var newObjectiveModifier = ReadObjectiveModifier(rawFileData);

            for (var j = 0; j < i; j++)
            {
                var existingObjectiveModifier = result[j];

                FileReadingException.ReaderAssert(!existingObjectiveModifier.MatchesBaseModifierData(newObjectiveModifier), "Redundant objective modifier!");
            }

            result[i] = newObjectiveModifier;
        }

        return result;
    }

    private static ObjectiveModifier ReadObjectiveModifier(RawLevelFileDataReader rawFileData)
    {
        uint rawObjectiveModifierId = rawFileData.Read8BitUnsignedInteger();
        var objectiveModifierType = (ObjectiveModifierType)rawObjectiveModifierId;

        return objectiveModifierType switch
        {
            ObjectiveModifierType.LimitSpecificSkillAssignments => CreateLimitSpecificSkillAssignmentsModifier(),
            ObjectiveModifierType.LimitTotalSkillAssignments => CreateLimitTotalSkillAssignmentsModifier(),

            _ => Helpers.ThrowUnknownEnumValueException<ObjectiveModifierType, ObjectiveModifier>(objectiveModifierType),
        };

        LimitSpecificSkillAssignmentsModifier CreateLimitSpecificSkillAssignmentsModifier()
        {
            int skillId = rawFileData.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(LemmingSkillConstants.IsValidLemmingSkillId(skillId), "Invalid skill id");

            int maxSkillAssignments = rawFileData.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(maxSkillAssignments < EngineConstants.InfiniteSkillCount, "Invalid skill limit quantity");

            return new LimitSpecificSkillAssignmentsModifier
            {
                SkillId = skillId,
                MaxSkillAssignments = maxSkillAssignments,
            };
        }

        LimitTotalSkillAssignmentsModifier CreateLimitTotalSkillAssignmentsModifier()
        {
            int maxTotalSkillAssignments = rawFileData.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(maxTotalSkillAssignments < EngineConstants.InfiniteSkillCount, "Invalid skill limit quantity");

            return new LimitTotalSkillAssignmentsModifier
            {
                MaxTotalSkillAssignments = maxTotalSkillAssignments
            };
        }
    }

    private TalismanData[] ReadTalismanData(RawLevelFileDataReader rawFileData)
    {
        int numberOfTalismanData = rawFileData.Read8BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<TalismanData>(numberOfTalismanData);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadTalismanDatum(rawFileData);
        }

        return result;
    }

    private TalismanData ReadTalismanDatum(RawLevelFileDataReader rawFileData)
    {
        int talismanNameId = rawFileData.Read16BitUnsignedInteger();

        var overrideObjectiveCriteria = ReadObjectiveCriteria(rawFileData);
        var overrideObjectiveModifiers = ReadObjectiveModifiers(rawFileData);

        return new TalismanData
        {
            TalismanName = _stringIdLookup[talismanNameId],

            OverrideObjectiveCriteria = overrideObjectiveCriteria,
            OverrideObjectiveModifiers = overrideObjectiveModifiers
        };
    }
}
