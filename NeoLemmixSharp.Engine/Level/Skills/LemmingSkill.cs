using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Skills;

public abstract class LemmingSkill : IExtendedEnumType<LemmingSkill>
{
    private static readonly LemmingSkill[] LemmingSkills = RegisterAllLemmingSkills();
    private static readonly LemmingSkillSet ClassicSkills = GetClassicSkills();
    protected static readonly LemmingActionSet ActionsThatCanBeAssignedPermanentSkill = GetActionsThatCanBeAssignedPermanentSkill();
    protected static readonly LemmingActionSet ActionsThatCanBeAssignedRotationSkill = GetActionsThatCanBeAssignedRotationSkill();

    public static int NumberOfItems => LemmingSkills.Length;
    public static ReadOnlySpan<LemmingSkill> AllItems => new(LemmingSkills);
    public static SimpleSetEnumerable<LemmingSkillComparer, LemmingSkill> AllClassicSkills => ClassicSkills.AsSimpleEnumerable();

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
            WaterLemmingSkill.Instance,

            FastForwardSkill.Instance
        };

        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<LemmingSkill>(result));
        Array.Sort(result, IdEquatableItemHelperMethods.Compare);

        return result;
    }

    private static LemmingSkillSet GetClassicSkills()
    {
        var result = CreateEmptySimpleSet();

        result.Add(ClimberSkill.Instance);
        result.Add(FloaterSkill.Instance);
        result.Add(BomberSkill.Instance);
        result.Add(BlockerSkill.Instance);
        result.Add(BuilderSkill.Instance);
        result.Add(BasherSkill.Instance);
        result.Add(MinerSkill.Instance);
        result.Add(DiggerSkill.Instance);

        return result;
    }

    private static LemmingActionSet GetActionsThatCanBeAssignedPermanentSkill()
    {
        var result = LemmingAction.CreateEmptySimpleSet();

        result.Add(AscenderAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(BlockerAction.Instance);
        result.Add(BuilderAction.Instance);
        result.Add(ClimberAction.Instance);
        result.Add(DehoisterAction.Instance);
        result.Add(DiggerAction.Instance);
        result.Add(DisarmerAction.Instance);
        result.Add(FallerAction.Instance);
        result.Add(FencerAction.Instance);
        result.Add(FloaterAction.Instance);
        result.Add(GliderAction.Instance);
        result.Add(HoisterAction.Instance);
        result.Add(JumperAction.Instance);
        result.Add(LasererAction.Instance);
        result.Add(MinerAction.Instance);
        result.Add(PlatformerAction.Instance);
        result.Add(ReacherAction.Instance);
        result.Add(ShimmierAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(SliderAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(SwimmerAction.Instance);
        result.Add(WalkerAction.Instance);
        result.Add(RotateClockwiseAction.Instance);
        result.Add(RotateCounterclockwiseAction.Instance);
        result.Add(RotateHalfAction.Instance);

        return result;
    }

    private static LemmingActionSet GetActionsThatCanBeAssignedRotationSkill()
    {
        var result = LemmingAction.CreateEmptySimpleSet();

        result.Add(WalkerAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(PlatformerAction.Instance);
        result.Add(BuilderAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(BasherAction.Instance);
        result.Add(FencerAction.Instance);
        result.Add(MinerAction.Instance);
        result.Add(DiggerAction.Instance);
        result.Add(LasererAction.Instance);

        return result;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LemmingSkillSet CreateEmptySimpleSet() => LemmingSkillComparer.CreateSimpleSet();

    private readonly LemmingActionSet _assignableActions;
    public readonly int Id;
    public readonly string LemmingSkillName;

    protected LemmingSkill(int id, string lemmingSkillName)
    {
        Id = id;
        LemmingSkillName = lemmingSkillName;

        _assignableActions = ActionsThatCanBeAssigned();
    }

    public bool IsClassicSkill() => ClassicSkills.Contains(this);

    public virtual bool CanAssignToLemming(Lemming lemming)
    {
        return ActionIsAssignable(lemming);
    }

    [Pure]
    protected abstract LemmingActionSet ActionsThatCanBeAssigned();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool ActionIsAssignable(Lemming lemming)
    {
        return _assignableActions.Contains(lemming.CurrentAction);
    }

    public abstract void AssignToLemming(Lemming lemming);

    int IIdEquatable<LemmingSkill>.Id => Id;
    public bool Equals(LemmingSkill? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is LemmingSkill other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => LemmingSkillName;

    public static bool operator ==(LemmingSkill left, LemmingSkill right) => left.Id == right.Id;
    public static bool operator !=(LemmingSkill left, LemmingSkill right) => left.Id != right.Id;

    [InlineArray((EngineConstants.NumberOfLemmingSkills + BitArrayHelpers.Mask) >> BitArrayHelpers.Shift)]
    public struct LemmingSkillBitBuffer
    {
        public uint _x;
    }
}