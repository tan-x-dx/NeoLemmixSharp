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

        int tribeId = rawFileData.Read8BitUnsignedInteger();
        tribeId--; // Need to offset by 1
        FileReadingException.ReaderAssert(tribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe id");

        int initialQuantity = rawFileData.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert(initialQuantity <= EngineConstants.InfiniteSkillCount, "Invalid skill count quantity");

        return new SkillSetData(skillId, tribeId, initialQuantity);
    }

    private static ObjectiveCriterionData[] ReadObjectiveCriteria(RawLevelFileDataReader rawFileData)
    {
        int numberOfObjectiveCriteria = rawFileData.Read8BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<ObjectiveCriterionData>(numberOfObjectiveCriteria);

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

    private static ObjectiveCriterionData ReadObjectiveCriterion(RawLevelFileDataReader rawFileData)
    {
        uint rawObjectiveCriterionId = rawFileData.Read8BitUnsignedInteger();
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
            int saveRequirement = rawFileData.Read16BitUnsignedInteger();
            int tribeId = rawFileData.Read8BitUnsignedInteger();
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
            int timeLimitInSeconds = rawFileData.Read16BitUnsignedInteger();
            FileReadingException.ReaderAssert(timeLimitInSeconds <= EngineConstants.MaxTimeLimitInSeconds, "Invalid time limit");

            return new TimeLimitCriterionData
            {
                TimeLimitInSeconds = timeLimitInSeconds
            };
        }
    }

    private static ObjectiveModifierData[] ReadObjectiveModifiers(RawLevelFileDataReader rawFileData)
    {
        int numberOfObjectiveModifiers = rawFileData.Read8BitUnsignedInteger();

        var result = Helpers.GetArrayForSize<ObjectiveModifierData>(numberOfObjectiveModifiers);

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

    private static ObjectiveModifierData ReadObjectiveModifier(RawLevelFileDataReader rawFileData)
    {
        uint rawObjectiveModifierId = rawFileData.Read8BitUnsignedInteger();
        var objectiveModifierType = (ObjectiveModifierType)rawObjectiveModifierId;

        return objectiveModifierType switch
        {
            ObjectiveModifierType.LimitSpecificSkillAssignments => CreateLimitSpecificSkillAssignmentsModifier(),
            ObjectiveModifierType.LimitTotalSkillAssignments => CreateLimitTotalSkillAssignmentsModifier(),

            _ => Helpers.ThrowUnknownEnumValueException<ObjectiveModifierType, ObjectiveModifierData>(objectiveModifierType),
        };

        LimitSpecificSkillAssignmentsModifierData CreateLimitSpecificSkillAssignmentsModifier()
        {
            int skillId = rawFileData.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(LemmingSkillConstants.IsValidLemmingSkillId(skillId), "Invalid skill id");

            int tribeId = rawFileData.Read8BitUnsignedInteger();
            tribeId--; // Need to offset by 1
            FileReadingException.ReaderAssert(tribeId < EngineConstants.MaxNumberOfTribes, "Invalid tribe id");

            int maxSkillAssignments = rawFileData.Read8BitUnsignedInteger();
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
            int maxTotalSkillAssignments = rawFileData.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(maxTotalSkillAssignments <= EngineConstants.MaxFiniteSkillCount, "Invalid skill limit quantity");

            return new LimitTotalSkillAssignmentsModifierData
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
        int talismanId = rawFileData.Read8BitUnsignedInteger();
        int talismanNameId = rawFileData.Read16BitUnsignedInteger();

        uint rawRankId = rawFileData.Read8BitUnsignedInteger();
        var rank = TalismanRankHelpers.GetEnumValue(rawRankId);

        var overrideObjectiveCriteria = ReadObjectiveCriteria(rawFileData);
        var overrideObjectiveModifiers = ReadObjectiveModifiers(rawFileData);

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
