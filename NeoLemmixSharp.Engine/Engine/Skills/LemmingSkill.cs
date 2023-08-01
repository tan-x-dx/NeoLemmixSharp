using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.Terrain;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Engine.Skills;

public abstract class LemmingSkill : IEquatable<LemmingSkill>, IUniqueIdItem
{
    private static readonly LemmingSkill[] LemmingSkills = RegisterAllLemmingSkills();
    protected static TerrainManager Terrain { get; private set; }

    public static ReadOnlySpan<LemmingSkill> AllLemmingSkills => new(LemmingSkills);

    private static LemmingSkill[] RegisterAllLemmingSkills()
    {
        var list = new List<LemmingSkill>();

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

        ListValidatorMethods.ValidateUniqueIds(list);

        list.Sort((x, y) => x.Id.CompareTo(y.Id));

        return list.ToArray();

        void RegisterLemmingSkill(LemmingSkill lemmingSkill)
        {
            if (lemmingSkill == NoneSkill.Instance)
                return;

            list.Add(lemmingSkill);
        }
    }

    public static void SetTerrain(TerrainManager terrain)
    {
        Terrain = terrain;
    }

    private readonly SimpleSet<LemmingAction> _assignableActions;

    protected LemmingSkill()
    {
        _assignableActions = new SimpleSet<LemmingAction>(SimpleLemmingActionHasher.Instance);

        // ReSharper disable once VirtualMemberCallInConstructor
        foreach (var action in ActionsThatCanBeAssigned())
        {
            _assignableActions.Add(action);
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
        return _assignableActions.Contains(lemming.CurrentAction);
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

public sealed class SimpleLemmingSkillHasher : ISimpleHasher<LemmingSkill>
{
    public static SimpleLemmingSkillHasher Instance { get; } = new();

    private SimpleLemmingSkillHasher()
    {
    }

    public int NumberOfItems => LemmingSkill.AllLemmingSkills.Length;

    public int Hash(LemmingSkill lemmingSkill) => lemmingSkill.Id;
    public LemmingSkill Unhash(int hash) => LemmingSkill.AllLemmingSkills[hash];
}