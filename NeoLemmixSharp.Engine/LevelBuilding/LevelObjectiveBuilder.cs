using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Objectives.Criteria;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Objectives;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LevelObjectiveBuilder
{
    private readonly LevelObjectiveData _levelObjectiveData;
    private readonly TalismanData? _selectedTalisman;

    private ObjectiveCriterionData[] TalismanCriteriaData => _selectedTalisman?.OverrideObjectiveCriteria ?? [];
    private ObjectiveModifierData[] TalismanModifierData => _selectedTalisman?.OverrideObjectiveModifiers ?? [];

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
        var levelObjectives = BuildObjectiveCriteria();

        return new LevelObjectiveManager(levelObjectives);
    }

    private ObjectiveRequirement[] BuildObjectiveCriteria()
    {
        var objectiveCriteriaList = new List<ObjectiveRequirement>();

        TryBuildSaveRequirementCriteria(objectiveCriteriaList);
        TryBuildTimeLimitCriterion(objectiveCriteriaList);
        TryBuildKillAllZombiesCriterion(objectiveCriteriaList);

        return objectiveCriteriaList.ToArray();
    }

    private void TryBuildSaveRequirementCriteria(List<ObjectiveRequirement> objectiveCriteriaList)
    {
        throw new NotImplementedException();
    }

    private void TryBuildTimeLimitCriterion(List<ObjectiveRequirement> objectiveCriteriaList)
    {
        var timeLimitCriterion = TryGetObjectiveCriterion<TimeLimitCriterionData>();

        if (timeLimitCriterion is null)
            return;

        objectiveCriteriaList.Add(new TimeRequirement());
    }

    private void TryBuildKillAllZombiesCriterion(List<ObjectiveRequirement> objectiveCriteriaList)
    {
        var killAllZombiesCriterion = TryGetObjectiveCriterion<KillAllZombiesCriterionData>();

        if (killAllZombiesCriterion is null)
            return;

        objectiveCriteriaList.Add(new AllZombiesDeadRequirement());
    }

    public SkillSetManager BuildSkillSetManager(TribeManager tribeManager)
    {
        var skillTrackingData = BuildSkillTrackingData(tribeManager);

        var totalSkillLimitModifier = TryGetObjectiveModifier<LimitTotalSkillAssignmentsModifierData>();
        var totalSkillLimit = EngineConstants.TrivialSkillLimit;
        if (totalSkillLimitModifier is not null)
            totalSkillLimit = totalSkillLimitModifier.MaxTotalSkillAssignments;

        return new SkillSetManager(skillTrackingData, totalSkillLimit);
    }

    private SkillTrackingData[] BuildSkillTrackingData(TribeManager tribeManager)
    {
        var baseSkillData = _levelObjectiveData.SkillSetData;
        var talismanModifiers = TalismanModifierData;

        var result = new SkillTrackingData[baseSkillData.Length];

        for (var i = 0; i < result.Length; i++)
        {
            var skillSetData = baseSkillData[i];

            var skillLimitModifier = TryGetAnySpecificSkillModifier(_levelObjectiveData.ObjectiveModifiers, talismanModifiers, skillSetData);
            var initialSkillLimit = EngineConstants.TrivialSkillLimit;
            if (skillLimitModifier is not null)
                initialSkillLimit = skillLimitModifier.MaxSkillAssignments;

            var skill = LemmingSkill.AllItems[skillSetData.SkillId];

            var tribe = skillSetData.TribeId == -1
                ? null
                : tribeManager.TryGetTribe(skillSetData.TribeId);

            result[i] = new SkillTrackingData(skill, tribe, i, skillSetData.InitialQuantity, initialSkillLimit);
        }

        return result;

        static LimitSpecificSkillAssignmentsModifierData? TryGetAnySpecificSkillModifier(
            ObjectiveModifierData[] baseModifiers,
            ObjectiveModifierData[] talismanModifiers,
            IO.Data.Level.Objectives.SkillSetData skillSetData)
        {
            return TryGetSpecificSkillModifier(talismanModifiers, skillSetData) ??
                   TryGetSpecificSkillModifier(baseModifiers, skillSetData);

        }

        static LimitSpecificSkillAssignmentsModifierData? TryGetSpecificSkillModifier(
            ObjectiveModifierData[] modifiers,
            IO.Data.Level.Objectives.SkillSetData skillSetData)
        {
            foreach (var modifierData in modifiers)
            {
                if (modifierData is LimitSpecificSkillAssignmentsModifierData skillLimit)
                {
                    if (skillLimit.SkillId == skillSetData.SkillId &&
                        skillLimit.TribeId == skillSetData.TribeId)
                        return skillLimit;
                }
            }

            return null;
        }
    }

    public LevelTimer BuildLevelTimer()
    {
        TimeLimitCriterionData? timeLimitCriterionData = TryGetObjectiveCriterion<TimeLimitCriterionData>();

        return timeLimitCriterionData is null
            ? LevelTimer.CreateCountUpTimer()
            : LevelTimer.CreateCountDownTimer(timeLimitCriterionData.TimeLimitInSeconds);
    }

    private TCriterionData? TryGetObjectiveCriterion<TCriterionData>()
        where TCriterionData : ObjectiveCriterionData
    {
        return TalismanCriteriaData.TryFindItemOfType<ObjectiveCriterionData, TCriterionData>() ??
               _levelObjectiveData.ObjectiveCriteria.TryFindItemOfType<ObjectiveCriterionData, TCriterionData>();
    }

    private TModifierData? TryGetObjectiveModifier<TModifierData>()
        where TModifierData : ObjectiveModifierData
    {
        return TalismanModifierData.TryFindItemOfType<ObjectiveModifierData, TModifierData>() ??
               _levelObjectiveData.ObjectiveModifiers.TryFindItemOfType<ObjectiveModifierData, TModifierData>();
    }

    private LimitTotalSkillAssignmentsModifierData? TryGetLimitTotalSkillAssignmentsModifier()
    {
        return TalismanModifierData.TryFindItemOfType<ObjectiveModifierData, LimitTotalSkillAssignmentsModifierData>() ??
               _levelObjectiveData.ObjectiveModifiers.TryFindItemOfType<ObjectiveModifierData, LimitTotalSkillAssignmentsModifierData>();
    }
}
