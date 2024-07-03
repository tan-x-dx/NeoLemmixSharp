using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Skills;

public abstract class LemmingSkill : IExtendedEnumType<LemmingSkill>
{
    private static readonly LemmingSkill[] LemmingSkills = RegisterAllLemmingSkills();

    public static int NumberOfItems => LemmingSkills.Length;
    public static ReadOnlySpan<LemmingSkill> AllItems => new(LemmingSkills);
    public static ReadOnlySpan<LemmingSkill> AllClassicSkills => new(LemmingSkills, LevelConstants.ClimberSkillId, LevelConstants.NumberOfClassicSkills);

    private static LemmingSkill[] RegisterAllLemmingSkills()
    {
        // NOTE: DO NOT ADD THE NONE SKILL
        var result = new LemmingSkill[]
        {
            ClimberSkill.Instance,
            FloaterSkill.Instance,
            BlockerSkill.Instance,
            BomberSkill.Instance,
            BuilderSkill.Instance,
            BasherSkill.Instance,
            MinerSkill.Instance,
            DiggerSkill.Instance,

            WalkerSkill.Instance,
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
            ClonerSkill.Instance,

            RotateClockwiseSkill.Instance,
            RotateCounterclockwiseSkill.Instance,
            RotateHalfSkill.Instance,
            RotateToDownSkill.Instance,
            RotateToRightSkill.Instance,
            RotateToUpSkill.Instance,
            RotateToLeftSkill.Instance,

            AcidLemmingSkill.Instance,
            WaterLemmingSkill.Instance
        };

        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<LemmingSkill>(result));
        Array.Sort(result, IdEquatableItemHelperMethods.Compare);

        return result;
    }

    private readonly SimpleSet<LemmingAction> _assignableActions;
    public readonly int Id;
    public readonly string LemmingSkillName;
    public readonly bool IsClassicSkill;

    protected LemmingSkill(int id, string lemmingSkillName, bool isClassicSkill)
    {
        Id = id;
        LemmingSkillName = lemmingSkillName;
        IsClassicSkill = isClassicSkill;
        _assignableActions = ExtendedEnumTypeComparer<LemmingAction>.CreateSimpleSet();

        // ReSharper disable once VirtualMemberCallInConstructor
        foreach (var action in ActionsThatCanBeAssigned())
        {
            _assignableActions.Add(action);
        }
    }

    public virtual bool CanAssignToLemming(Lemming lemming)
    {
        return ActionIsAssignable(lemming);
    }

    protected abstract IEnumerable<LemmingAction> ActionsThatCanBeAssigned();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool ActionIsAssignable(Lemming lemming)
    {
        return _assignableActions.Contains(lemming.CurrentAction);
    }

    public abstract void AssignToLemming(Lemming lemming);

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

        yield return RotateClockwiseAction.Instance;
        yield return RotateCounterclockwiseAction.Instance;
        yield return RotateHalfAction.Instance;
    }

    int IIdEquatable<LemmingSkill>.Id => Id;
    public bool Equals(LemmingSkill? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is LemmingSkill other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => LemmingSkillName;

    public static bool operator ==(LemmingSkill left, LemmingSkill right) => left.Id == right.Id;
    public static bool operator !=(LemmingSkill left, LemmingSkill right) => left.Id != right.Id;
}