using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Tribes;

namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class SkillTrackingData
{
    private readonly SkillSetData _data;

    public int LemmingSkillId { get; }
    public LemmingSkill LemmingSkill => LemmingSkill.GetSkillOrDefault(LemmingSkillId);

    public int TribeId { get; }
    public Tribe? Tribe => LevelScreen.TribeManager.GetTribeOrDefault(TribeId);

    public int SkillTrackingDataId { get; }
    public int InitialSkillQuantity { get; }

    public int EffectiveQuantity { get; private set; }

    public bool IsInfinite => InitialSkillQuantity == EngineConstants.InfiniteSkillCount;

    public SkillTrackingData(
        ref nint dataHandle,
        int lemmingSkillId,
        int tribeId,
        int skillTrackingDataId,
        int initialSkillQuantity,
        int initialSkillLimit)
    {
        _data = PointerDataHelper.CreateItem<SkillSetData>(ref dataHandle);

        LemmingSkillId = lemmingSkillId;
        TribeId = tribeId;
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
               LemmingSkill.CanAssignToLemming(lemming);
    }

    private bool TribesMatch(Lemming lemming)
    {
        var thisTribe = Tribe;
        if (thisTribe is null)
            return true;

        var lemmingTribeId = lemming.State.TribeId;
        var lemmingTribe = LevelScreen.TribeManager.GetTribe(lemmingTribeId);

        return thisTribe.Equals(lemmingTribe);
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
}
