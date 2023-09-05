using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Skills;

public abstract class LemmingSkill : IUniqueIdItem<LemmingSkill>
{
    private static readonly LemmingSkill[] LemmingSkills = RegisterAllLemmingSkills();
    protected static TerrainManager Terrain { get; private set; }

    public static int NumberOfItems => LemmingSkills.Length;
    public static ReadOnlySpan<LemmingSkill> AllItems => new(LemmingSkills);

    private static LemmingSkill[] RegisterAllLemmingSkills()
    {
        // NOTE: DO NOT ADD THE NONE SKILL
        var result = new LemmingSkill[]
        {
            WalkerSkill.Instance,
            ClimberSkill.Instance,
            FloaterSkill.Instance,
            BlockerSkill.Instance,
            BomberSkill.Instance,
            BuilderSkill.Instance,
            BasherSkill.Instance,
            MinerSkill.Instance,
            DiggerSkill.Instance,

            PlatformerSkill.Instance,
            StackerSkill.Instance,
            FencerSkill.Instance,
            GliderSkill.Instance,
            JumperSkill.Instance,
            SwimmerSkill.Instance,
            ShimmierSkill.Instance,
            LasererSkill.Instance,
            SliderSkill.Instance,
            DisarmerSkill.Instance,
            StonerSkill.Instance,
            ClonerSkill.Instance
        };

        result.ValidateUniqueIds();
        Array.Sort(result, UniqueIdItemComparer<LemmingSkill>.Instance);

        return result;
    }

    public static void SetTerrain(TerrainManager terrain)
    {
        Terrain = terrain;
    }

    private readonly LargeSimpleSet<LemmingAction> _assignableActions;

    protected LemmingSkill()
    {
        _assignableActions = SimpleSetHelpers.LargeSetForUniqueItemType<LemmingAction>();

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