using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Engine.Actions;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public abstract class LemmingSkill : IEquatable<LemmingSkill>
{
    protected static TerrainManager Terrain { get; private set; }

    private static ReadOnlyDictionaryWrapper<string, LemmingSkill> LemmingSkills { get; } = RegisterAllLemmingSkills();

    private static ReadOnlyDictionaryWrapper<string, LemmingSkill> RegisterAllLemmingSkills()
    {
        var result = new Dictionary<string, LemmingSkill>();

        // NOTE: DO NOT REGISTER THE NONE SKILL

        RegisterLemmingSkill(WalkerSkill.Instance);
        RegisterLemmingSkill(ClimberSkill.Instance);
        RegisterLemmingSkill(FloaterSkill.Instance);
        RegisterLemmingSkill(BlockerSkill.Instance);
        RegisterLemmingSkill(BomberSkill.Instance);
        RegisterLemmingSkill(BuilderSkill.Instance);
        RegisterLemmingSkill(BasherSkill.Instance);
        RegisterLemmingSkill(MinerSkill.Instance);
        RegisterLemmingSkill(DiggerSkill.Instance);

        RegisterLemmingSkill(PlatformerSkill.Instance);
        RegisterLemmingSkill(StackerSkill.Instance);
        RegisterLemmingSkill(FencerSkill.Instance);
        RegisterLemmingSkill(GliderSkill.Instance);
        RegisterLemmingSkill(JumperSkill.Instance);
        RegisterLemmingSkill(SwimmerSkill.Instance);
        RegisterLemmingSkill(ShimmierSkill.Instance);
        RegisterLemmingSkill(LasererSkill.Instance);
        RegisterLemmingSkill(SliderSkill.Instance);
        RegisterLemmingSkill(DisarmerSkill.Instance);
        RegisterLemmingSkill(StonerSkill.Instance);

        RegisterLemmingSkill(ClonerSkill.Instance);

        ValidateLemmingSkillIds();

        return new ReadOnlyDictionaryWrapper<string, LemmingSkill>(result);

        void RegisterLemmingSkill(LemmingSkill lemmingSkill)
        {
            if (lemmingSkill == NoneSkill.Instance)
                return;

            result.Add(lemmingSkill.LemmingSkillName, lemmingSkill);
        }

        void ValidateLemmingSkillIds()
        {
            var ids = result
                .Values
                .Select(ls => ls.Id)
                .ToList();

            var numberOfUniqueIds = ids
                .Distinct()
                .Count();

            if (numberOfUniqueIds != result.Count)
            {
                var idsString = ids.OrderBy(i => i);

                throw new Exception($"Duplicated skill ID: {idsString}");
            }

            var minSkillId = ids.Min();
            var maxSkillId = ids.Max();

            if (minSkillId != 0 || maxSkillId != result.Count - 1)
                throw new Exception($"Skill ids do not span a full set of values from 0 - {result.Count - 1}");
        }
    }

    public static ICollection<LemmingSkill> AllLemmingSkills => LemmingSkills.Values;

    public static void SetTerrain(TerrainManager terrain)
    {
        Terrain = terrain;
    }

    private readonly IBitArray _assignableActionIds;

    protected LemmingSkill()
    {
        var numberOfActions = LemmingAction.AllActions.Count;

        _assignableActionIds = IBitArray.GetBestFitForSize(numberOfActions);

        // ReSharper disable once VirtualMemberCallInConstructor
        foreach (var action in ActionsThatCanBeAssigned())
        {
            _assignableActionIds.SetBit(action.Id);
        }
    }

    public abstract int Id { get; }
    public abstract string LemmingSkillName { get; }
    public abstract bool IsPermanentSkill { get; }
    public abstract bool IsClassicSkill { get; }

    public virtual bool CanAssignToLemming(Lemming lemming)
    {
        return ActionIsAssignable(lemming);
    }

    protected abstract IEnumerable<LemmingAction> ActionsThatCanBeAssigned();

    [Pure]
    protected bool ActionIsAssignable(Lemming lemming)
    {
        return _assignableActionIds.GetBit(lemming.CurrentAction.Id);
    }

    public abstract bool AssignToLemming(Lemming lemming);

    public bool Equals(LemmingSkill? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is LemmingSkill other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => LemmingSkillName;

    public static bool operator ==(LemmingSkill left, LemmingSkill right) => left.Id == right.Id;
    public static bool operator !=(LemmingSkill left, LemmingSkill right) => left.Id != right.Id;

    protected static IEnumerable<LemmingAction> ActionsThatCanBeAssignedPermanentSkill()
    {
        yield return AscenderAction.Instance;
        yield return BasherAction.Instance;
        yield return BlockerAction.Instance;
        yield return BuilderAction.Instance;
        yield return ClimberAction.Instance;
        yield return DehoisterAction.Instance;
        yield return DiggerAction.Instance;
        yield return DisarmerAction.Instance;
        yield return FallerAction.Instance;
        yield return FencerAction.Instance;
        yield return FloaterAction.Instance;
        yield return GliderAction.Instance;
        yield return HoisterAction.Instance;
        yield return JumperAction.Instance;
        yield return LasererAction.Instance;
        yield return MinerAction.Instance;
        yield return PlatformerAction.Instance;
        yield return ReacherAction.Instance;
        yield return ShimmierAction.Instance;
        yield return ShruggerAction.Instance;
        yield return SliderAction.Instance;
        yield return StackerAction.Instance;
        yield return SwimmerAction.Instance;
        yield return WalkerAction.Instance;
    }
}