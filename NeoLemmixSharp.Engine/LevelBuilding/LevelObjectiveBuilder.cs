using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Objectives.Criteria;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Objectives;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelObjectiveBuilder
{
    private readonly LevelObjectiveData _levelObjectiveData;
    private readonly TalismanData? _selectedTalisman;

    public RawArray SkillSetDataBuffer { get; private set; }
    public RawArray LevelTimerDataBuffer { get; private set; }

    public LevelObjectiveBuilder(LevelObjectiveData levelObjective, int selectedTalismanId)
    {
        _levelObjectiveData = levelObjective;

        _selectedTalisman = TryGetSelectedTalisman(levelObjective, selectedTalismanId);
    }

    private static TalismanData? TryGetSelectedTalisman(LevelObjectiveData levelObjective, int selectedTalismanId)
    {
        if (selectedTalismanId == -1)
            return null;

        return levelObjective.TalismanData[selectedTalismanId];
    }

    public LevelObjectiveManager BuildLevelObjectiveManager()
    {
        var baseObjectiveRequirements = BuildBaseObjectiveRequirements();
        var mainLevelObjective = new LevelObjective(baseObjectiveRequirements, _levelObjectiveData.ObjectiveName);

        var talismanObjectives = BuildTalismanObjectives();
        LevelObjective? selectedTalismanObjective = null;
        if (_selectedTalisman is not null)
            selectedTalismanObjective = talismanObjectives[_selectedTalisman.TalismanId];

        return new LevelObjectiveManager(mainLevelObjective, talismanObjectives, selectedTalismanObjective);
    }

    private ObjectiveRequirement[] BuildBaseObjectiveRequirements()
    {
        var objectiveCriteriaList = new List<ObjectiveRequirement>(3);

        if (TryBuildSaveRequirementCriteria(_levelObjectiveData.ObjectiveCriteria, out var requirement)) objectiveCriteriaList.Add(requirement);
        if (TryBuildTimeLimitCriterion(_levelObjectiveData.ObjectiveCriteria, out requirement)) objectiveCriteriaList.Add(requirement);
        if (TryBuildKillAllZombiesCriterion(_levelObjectiveData.ObjectiveCriteria, out requirement)) objectiveCriteriaList.Add(requirement);

        return objectiveCriteriaList.ToArray();
    }

    private LevelObjective[] BuildTalismanObjectives()
    {
        if (_levelObjectiveData.TalismanData.Length == 0)
            return [];

        var result = new LevelObjective[_levelObjectiveData.TalismanData.Length];

        for (var i = 0; i < _levelObjectiveData.TalismanData.Length; i++)
        {
            var talismanData = _levelObjectiveData.TalismanData[i];
            result[i] = BuildTalismanObjective(talismanData);
        }

        return result;
    }

    private static LevelObjective BuildTalismanObjective(TalismanData talismanData)
    {
        var objectiveCriteriaList = new List<ObjectiveRequirement>();

        if (TryBuildSaveRequirementCriteria(talismanData.AdditionalObjectiveCriteria, out var requirement)) objectiveCriteriaList.Add(requirement);
        if (TryBuildTimeLimitCriterion(talismanData.AdditionalObjectiveCriteria, out requirement)) objectiveCriteriaList.Add(requirement);
        if (TryBuildKillAllZombiesCriterion(talismanData.AdditionalObjectiveCriteria, out requirement)) objectiveCriteriaList.Add(requirement);

        return new LevelObjective(objectiveCriteriaList.ToArray(), talismanData.TalismanName);
    }

    private static bool TryBuildSaveRequirementCriteria(ObjectiveCriterionData[] criteria, [MaybeNullWhen(false)] out ObjectiveRequirement saveRequirement)
    {
        var saveRequirementCriterion = criteria.TryFindItemOfType<ObjectiveCriterionData, SaveLemmingsCriterionData>();
        if (saveRequirementCriterion is null)
        {
            saveRequirement = null;
            return false;
        }

        saveRequirement = new SaveRequirement(saveRequirementCriterion.SaveRequirement, saveRequirementCriterion.TribeId);
        return true;
    }

    private static bool TryBuildTimeLimitCriterion(ObjectiveCriterionData[] criteria, [MaybeNullWhen(false)] out ObjectiveRequirement timeRequirement)
    {
        var timeLimitCriterion = criteria.TryFindItemOfType<ObjectiveCriterionData, TimeLimitCriterionData>();

        if (timeLimitCriterion is null)
        {
            timeRequirement = null;
            return false;
        }

        timeRequirement = new TimeRequirement(timeLimitCriterion.TimeLimitInSeconds);
        return true;
    }

    private static bool TryBuildKillAllZombiesCriterion(ObjectiveCriterionData[] criteria, [MaybeNullWhen(false)] out ObjectiveRequirement killAllZombiesRequirement)
    {
        var killAllZombiesCriterion = criteria.TryFindItemOfType<ObjectiveCriterionData, KillAllZombiesCriterionData>();

        if (killAllZombiesCriterion is null)
        {
            killAllZombiesRequirement = null;
            return false;
        }

        killAllZombiesRequirement = new AllZombiesDeadRequirement();
        return true;
    }

    public SkillSetManager BuildSkillSetManager(TribeManager tribeManager, SafeBufferAllocator safeBufferAllocator)
    {
        SkillSetDataBuffer = safeBufferAllocator.AllocateRawArray(PointerWrapper.SizeInBytes + (_levelObjectiveData.SkillSetData.Length * Level.Objectives.SkillSetData.SizeInBytes));

        var skillTrackingData = BuildSkillTrackingData(tribeManager);

        var totalSkillLimitModifier = TryFindSmallestLimitTotalSkillAssignmentsModifier();
        var totalSkillLimit = EngineConstants.TrivialSkillLimit;
        if (totalSkillLimitModifier is not null)
            totalSkillLimit = totalSkillLimitModifier.MaxTotalSkillAssignments;

        return new SkillSetManager(SkillSetDataBuffer.Handle, skillTrackingData, totalSkillLimit);
    }

    private SkillTrackingData[] BuildSkillTrackingData(TribeManager tribeManager)
    {
        var baseSkillData = _levelObjectiveData.SkillSetData;

        var result = new SkillTrackingData[baseSkillData.Length];

        nint handle = SkillSetDataBuffer.Handle + PointerWrapper.SizeInBytes;

        for (var i = 0; i < result.Length; i++)
        {
            var skillSetData = baseSkillData[i];

            var skillLimitModifier = TryGetAnySpecificSkillModifier(_levelObjectiveData.ObjectiveModifiers, _selectedTalisman?.AdditionalObjectiveModifiers, skillSetData);
            var initialSkillLimit = EngineConstants.TrivialSkillLimit;
            if (skillLimitModifier is not null)
                initialSkillLimit = skillLimitModifier.MaxSkillAssignments;

            var skill = LemmingSkill.GetSkillOrDefault(skillSetData.SkillId);

            var tribe = skillSetData.TribeId == -1
                ? null
                : tribeManager.TryGetTribe(skillSetData.TribeId);

            result[i] = new SkillTrackingData(ref handle, skill, tribe, i, skillSetData.InitialQuantity, initialSkillLimit);
        }

        return result;
    }

    public LevelTimer BuildLevelTimer(SafeBufferAllocator safeBufferAllocator)
    {
        var timeLimitCriterionData = TryFindSmallestTimeLimitCriterion();

        LevelTimerDataBuffer = safeBufferAllocator.AllocateRawArray(LevelTimerData.SizeInBytes);

        return timeLimitCriterionData is null
            ? LevelTimer.CreateCountUpTimer(LevelTimerDataBuffer.Handle)
            : LevelTimer.CreateCountDownTimer(LevelTimerDataBuffer.Handle, timeLimitCriterionData.TimeLimitInSeconds);
    }

    private TimeLimitCriterionData? TryFindSmallestTimeLimitCriterion()
    {
        return Helpers.MaybeConcat(_selectedTalisman?.AdditionalObjectiveCriteria, _levelObjectiveData.ObjectiveCriteria)
            .OfType<TimeLimitCriterionData>()
            .MinBy(t => t.TimeLimitInSeconds);
    }

    private LimitTotalSkillAssignmentsModifierData? TryFindSmallestLimitTotalSkillAssignmentsModifier()
    {
        return Helpers.MaybeConcat(_selectedTalisman?.AdditionalObjectiveModifiers, _levelObjectiveData.ObjectiveModifiers)
            .OfType<LimitTotalSkillAssignmentsModifierData>()
            .MinBy(l => l.MaxTotalSkillAssignments);
    }

    private static LimitSpecificSkillAssignmentsModifierData? TryGetAnySpecificSkillModifier(
        ObjectiveModifierData[] baseModifiers,
        ObjectiveModifierData[]? talismanModifiers,
        IO.Data.Level.Objectives.SkillSetData skillSetData)
    {
        return Helpers
            .MaybeConcat(talismanModifiers, baseModifiers)
            .OfType<LimitSpecificSkillAssignmentsModifierData>()
            .FirstOrDefault(skillLimit => skillLimit.SkillId == skillSetData.SkillId && skillLimit.TribeId == skillSetData.TribeId);
    }
}
