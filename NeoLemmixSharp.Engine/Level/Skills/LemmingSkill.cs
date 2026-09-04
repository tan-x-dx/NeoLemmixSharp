using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Skills;

public abstract class LemmingSkill : IEquatable<LemmingSkill>
{
    protected static readonly LemmingActionTypeSet ActionsThatCanBeAssignedPermanentSkill = GetActionsThatCanBeAssignedPermanentSkill();
    protected static readonly LemmingActionTypeSet ActionsThatCanBeAssignedRotationSkill = GetActionsThatCanBeAssignedRotationSkill();
    private static readonly LemmingSkill[] LemmingSkills = RegisterAllLemmingSkills();
    private static readonly LemmingSkillSet ClassicSkills = GetClassicSkills();
    private static readonly LemmingSkillSet PermanentSkills = GetPermanentSkills();

    public static ReadOnlySpan<LemmingSkill> AllItems => new(LemmingSkills);
    public static BitArrayEnumerable<LemmingSkillHasher, LemmingSkill> AllClassicSkills => ClassicSkills.AsEnumerable();

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

            AcidLemmingSkill.Instance,
            WaterLemmingSkill.Instance,

            FastForwardSkill.Instance
        };

        Debug.Assert(result.Length == LemmingSkillConstants.NumberOfLemmingSkills);

        var hasher = new LemmingSkillHasher();
        hasher.AssertUniqueIds(new ReadOnlySpan<LemmingSkill>(result));
        Array.Sort(result, hasher);

        return result;
    }

    private static LemmingSkillSet GetClassicSkills()
    {
        var result = CreateBitArraySet();

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

    private static LemmingSkillSet GetPermanentSkills()
    {
        var result = CreateBitArraySet();

        result.Add(ClimberSkill.Instance);
        result.Add(FloaterSkill.Instance);
        result.Add(GliderSkill.Instance);
        result.Add(SliderSkill.Instance);
        result.Add(SwimmerSkill.Instance);
        result.Add(DisarmerSkill.Instance);
        result.Add(AcidLemmingSkill.Instance);
        result.Add(WaterLemmingSkill.Instance);
        result.Add(FastForwardSkill.Instance);

        return result;
    }

    private static LemmingActionTypeSet GetActionsThatCanBeAssignedPermanentSkill()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(LemmingActionType.AscenderAction);
        result.Add(LemmingActionType.BasherAction);
        result.Add(LemmingActionType.BlockerAction);
        result.Add(LemmingActionType.BuilderAction);
        result.Add(LemmingActionType.ClimberAction);
        result.Add(LemmingActionType.DehoisterAction);
        result.Add(LemmingActionType.DiggerAction);
        result.Add(LemmingActionType.DisarmerAction);
        result.Add(LemmingActionType.FallerAction);
        result.Add(LemmingActionType.FencerAction);
        result.Add(LemmingActionType.FloaterAction);
        result.Add(LemmingActionType.GliderAction);
        result.Add(LemmingActionType.HoisterAction);
        result.Add(LemmingActionType.JumperAction);
        result.Add(LemmingActionType.LasererAction);
        result.Add(LemmingActionType.MinerAction);
        result.Add(LemmingActionType.PlatformerAction);
        result.Add(LemmingActionType.ReacherAction);
        result.Add(LemmingActionType.RotateHalfAction);
        result.Add(LemmingActionType.RotateClockwiseAction);
        result.Add(LemmingActionType.RotateCounterclockwiseAction);
        result.Add(LemmingActionType.ShimmierAction);
        result.Add(LemmingActionType.ShruggerAction);
        result.Add(LemmingActionType.SliderAction);
        result.Add(LemmingActionType.StackerAction);
        result.Add(LemmingActionType.SwimmerAction);
        result.Add(LemmingActionType.WalkerAction);

        return result;
    }

    private static LemmingActionTypeSet GetActionsThatCanBeAssignedRotationSkill()
    {
        var result = LemmingAction.CreateBitArraySet();

        result.Add(LemmingActionType.WalkerAction);
        result.Add(LemmingActionType.ShruggerAction);
        result.Add(LemmingActionType.PlatformerAction);
        result.Add(LemmingActionType.BuilderAction);
        result.Add(LemmingActionType.StackerAction);
        result.Add(LemmingActionType.BasherAction);
        result.Add(LemmingActionType.FencerAction);
        result.Add(LemmingActionType.MinerAction);
        result.Add(LemmingActionType.DiggerAction);
        result.Add(LemmingActionType.LasererAction);

        return result;
    }

    /// <summary>
    /// Safe alternative to performing the array lookup - the input may be negative, or an invalid lemming skill type. In such a case the <see cref="NoneSkill"/> is returned.
    /// </summary>
    /// <param name="lemmingSkillType">The (possibly invalid) type of the skill to fetch.</param>
    /// <returns>The LemmingSkill with that type, or the <see cref="NoneSkill"/> if the input is invalid.</returns>
    public static LemmingSkill GetSkillOrDefault(LemmingSkillType lemmingSkillType)
    {
        return (uint)lemmingSkillType < LemmingSkillConstants.NumberOfLemmingSkills
            ? LemmingSkills.At((int)lemmingSkillType)
            : NoneSkill.Instance;
    }

    private readonly LemmingActionTypeSet _assignableActions;
    public string LemmingSkillName { get; }
    public LemmingSkillType SkillType { get; }

    protected LemmingSkill(LemmingSkillType skillType, string lemmingSkillName)
    {
        SkillType = skillType;
        LemmingSkillName = lemmingSkillName;

        _assignableActions = ActionsThatCanBeAssigned();
    }

    public bool IsClassicSkill() => ClassicSkills.Contains(this);
    public bool IsPermanentSkill() => PermanentSkills.Contains(this);

    public virtual bool CanAssignToLemming(Lemming lemming)
    {
        return SkillIsAssignableToCurrentAction(lemming);
    }

    [Pure]
    protected abstract LemmingActionTypeSet ActionsThatCanBeAssigned();

    [Pure]
    protected bool SkillIsAssignableToCurrentAction(Lemming lemming)
    {
        return _assignableActions.Contains(lemming.CurrentActionType);
    }

    public abstract void AssignToLemming(Lemming lemming);

    [DebuggerStepThrough]
    public bool Equals(LemmingSkill? other)
    {
        var otherValue = LemmingSkillType.NoneSkill;
        if (other is not null) otherValue = other.SkillType;
        return SkillType == otherValue;
    }

    [DebuggerStepThrough]
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is LemmingSkill other && SkillType == other.SkillType;
    [DebuggerStepThrough]
    public sealed override int GetHashCode() => (int)SkillType;
    [DebuggerStepThrough]
    public sealed override string ToString() => LemmingSkillName;

    [DebuggerStepThrough]
    public static bool operator ==(LemmingSkill left, LemmingSkill right) => left.SkillType == right.SkillType;
    [DebuggerStepThrough]
    public static bool operator !=(LemmingSkill left, LemmingSkill right) => left.SkillType != right.SkillType;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LemmingSkillSet CreateBitArraySet() => new(new LemmingSkillHasher());
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<LemmingSkillHasher, LemmingSkillBitBuffer, LemmingSkill, TValue> CreateBitArrayDictionary<TValue>() => new(new LemmingSkillHasher());

    public readonly struct LemmingSkillHasher : IBitBufferCreator<LemmingSkillBitBuffer, LemmingSkill>
    {
        [Pure]
        public int NumberOfItems => LemmingSkillConstants.NumberOfLemmingSkills;
        [Pure]
        public int Hash(LemmingSkill item) => (int)item.SkillType;
        [Pure]
        public LemmingSkill UnHash(int index) => LemmingSkills.At(index);

        public void CreateBitBuffer(out LemmingSkillBitBuffer buffer) => buffer = new();
    }

    [InlineArray(LemmingSkillBitBufferLength)]
    public struct LemmingSkillBitBuffer : IBitBuffer
    {
        private const int LemmingSkillBitBufferLength = (LemmingSkillConstants.NumberOfLemmingSkills + BitArrayHelpers.Mask) >>> BitArrayHelpers.Shift;

        private uint _0;

        public readonly int Length => LemmingSkillBitBufferLength;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, LemmingSkillBitBufferLength);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, LemmingSkillBitBufferLength);
    }
}
