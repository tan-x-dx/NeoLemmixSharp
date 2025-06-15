using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Tribes;

namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class SkillTrackingData : ISnapshotDataConvertible<SkillSetSnapshotData>
{
    public LemmingSkill Skill { get; }
    public Tribe? Tribe { get; }

    public int SkillTrackingDataId { get; }
    public int InitialSkillQuantity { get; }

    private int _additionalQuantity;
    private int _amountUsed;
    private int _currentSkillLimit;

    public int EffectiveQuantity { get; private set; }

    public bool IsInfinite => InitialSkillQuantity == EngineConstants.InfiniteSkillCount;

    public SkillTrackingData(
        LemmingSkill skill,
        Tribe? tribe,
        int skillTrackingDataId,
        int initialSkillQuantity,
        int initialSkillLimit)
    {
        Skill = skill;
        Tribe = tribe;
        SkillTrackingDataId = skillTrackingDataId;
        InitialSkillQuantity = Math.Min(initialSkillQuantity, initialSkillLimit);
        _currentSkillLimit = initialSkillLimit;
    }

    public void UseSkill()
    {
        _amountUsed++;
        _currentSkillLimit--;

        var skillSetManager = LevelScreen.SkillSetManager;
        skillSetManager.RecordUsageOfSkill();
        skillSetManager.UpdateSkillSetData();
    }

    public void ChangeSkillCount(int delta)
    {
        _additionalQuantity += delta;

        LevelScreen.SkillSetManager.UpdateSkillSetData();
    }

    public bool CanAssignToLemming(Lemming lemming)
    {
        return EffectiveQuantity > 0 &&
               lemming.State.CanHaveSkillsAssigned &&
               TribesMatch(lemming) &&
               Skill.CanAssignToLemming(lemming);
    }

    private bool TribesMatch(Lemming lemming)
    {
        return Tribe is null ||
               Tribe == lemming.State.TribeAffiliation;
    }

    public void RecalculateEffectiveQuantity(int totalSkillLimit)
    {
        int effectiveQuantity;

        if (InitialSkillQuantity == EngineConstants.InfiniteSkillCount)
        {
            effectiveQuantity = EngineConstants.InfiniteSkillCount;
        }
        else
        {
            effectiveQuantity = InitialSkillQuantity + _additionalQuantity - _amountUsed;

            effectiveQuantity = Math.Min(effectiveQuantity, _currentSkillLimit);
            effectiveQuantity = Math.Clamp(effectiveQuantity, 0, EngineConstants.MaxFiniteSkillCount);
        }

        EffectiveQuantity = Math.Min(effectiveQuantity, totalSkillLimit);
    }

    public void WriteToSnapshotData(out SkillSetSnapshotData snapshotData)
    {
        snapshotData = new SkillSetSnapshotData(SkillTrackingDataId, _additionalQuantity, _amountUsed, _currentSkillLimit);
    }

    public void SetFromSnapshotData(in SkillSetSnapshotData snapshotData)
    {
        _additionalQuantity = snapshotData.AdditionalQuantity;
        _amountUsed = snapshotData.AmountUsed;
        _currentSkillLimit = snapshotData.CurrentSkillLimit;
    }
}
