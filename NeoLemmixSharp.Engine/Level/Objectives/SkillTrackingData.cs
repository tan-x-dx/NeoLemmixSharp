using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Tribes;

namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class SkillTrackingData : ISnapshotDataConvertible
{
    public LemmingSkill Skill { get; }
    public Tribe? Tribe { get; }

    public int SkillTrackingDataId { get; }
    public int InitialSkillQuantity { get; }

    private SkillSetData _data = new();

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
        _data.CurrentSkillLimit = initialSkillLimit;
    }

    public void UseSkill()
    {
        _data.AmountUsed++;
        _data.CurrentSkillLimit--;

        var skillSetManager = LevelScreen.SkillSetManager;
        skillSetManager.RecordUsageOfSkill();
        skillSetManager.UpdateSkillSetData();
    }

    public void ChangeSkillCount(int delta)
    {
        _data.AdditionalQuantity += delta;

        LevelScreen.SkillSetManager.UpdateSkillSetData();
    }

    public bool CanAssignToLemming(Lemming lemming)
    {
        return EffectiveQuantity > 0 &&
               TribesMatch(lemming) &&
               lemming.State.CanHaveSkillsAssigned &&
               Skill.CanAssignToLemming(lemming);
    }

    private bool TribesMatch(Lemming lemming)
    {
        return Tribe is null ||
               Tribe.Equals(lemming.State.TribeAffiliation);
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
            effectiveQuantity = InitialSkillQuantity + _data.AdditionalQuantity - _data.AmountUsed;
            effectiveQuantity = Math.Min(effectiveQuantity, EngineConstants.MaxFiniteSkillCount);
            effectiveQuantity = Math.Min(effectiveQuantity, _data.CurrentSkillLimit);
            effectiveQuantity = Math.Max(effectiveQuantity, 0);
        }

        EffectiveQuantity = Math.Min(effectiveQuantity, totalSkillLimit);
    }

    public unsafe int GetRequiredNumberOfBytesForSnapshotting() => sizeof(SkillSetData);

    public unsafe void WriteToSnapshotData(byte* snapshotDataPointer)
    {
        SkillSetData* skillSetSnapshotDataPointer = (SkillSetData*)snapshotDataPointer;

        *skillSetSnapshotDataPointer = _data;
    }

    public unsafe void SetFromSnapshotData(byte* snapshotDataPointer)
    {
        SkillSetData* skillSetSnapshotDataPointer = (SkillSetData*)snapshotDataPointer;

        _data = *skillSetSnapshotDataPointer;
    }
}
