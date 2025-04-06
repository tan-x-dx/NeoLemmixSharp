using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Skills;

public abstract class LemmingSkill : IIdEquatable<LemmingSkill>
{
    private static readonly LemmingSkill[] LemmingSkills = RegisterAllLemmingSkills();
    private static readonly LemmingSkillSet ClassicSkills = GetClassicSkills();
    protected static readonly LemmingActionSet ActionsThatCanBeAssignedPermanentSkill = GetActionsThatCanBeAssignedPermanentSkill();
    protected static readonly LemmingActionSet ActionsThatCanBeAssignedRotationSkill = GetActionsThatCanBeAssignedRotationSkill();

    public const int NumberOfItems = EngineConstants.NumberOfLemmingSkills;
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
            RotateToDownSkill.Instance,
            RotateToRightSkill.Instance,
            RotateToUpSkill.Instance,
            RotateToLeftSkill.Instance,

            AcidLemmingSkill.Instance,
            WaterLemmingSkill.Instance,

            FastForwardSkill.Instance
        };

        var hasher = new LemmingSkillHasher();
        hasher.ValidateUniqueIds(new ReadOnlySpan<LemmingSkill>(result));
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

    private static LemmingActionSet GetActionsThatCanBeAssignedPermanentSkill()
    {
        var result = LemmingAction.CreateBitArraySet();

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
        result.Add(RotateHalfAction.Instance);
        result.Add(RotateClockwiseAction.Instance);
        result.Add(RotateCounterclockwiseAction.Instance);
        result.Add(ShimmierAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(SliderAction.Instance);
        result.Add(StackerAction.Instance);
        result.Add(SwimmerAction.Instance);
        result.Add(WalkerAction.Instance);

        return result;
    }

    private static LemmingActionSet GetActionsThatCanBeAssignedRotationSkill()
    {
        var result = LemmingAction.CreateBitArraySet();

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

    private readonly LemmingActionSet _assignableActions;
    public readonly string LemmingSkillName;
    public readonly int Id;

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
    [DebuggerStepThrough]
    public bool Equals(LemmingSkill? other) => Id == (other?.Id ?? -1);
    [DebuggerStepThrough]
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is LemmingSkill other && Id == other.Id;
    [DebuggerStepThrough]
    public sealed override int GetHashCode() => Id;
    [DebuggerStepThrough]
    public sealed override string ToString() => LemmingSkillName;

    [DebuggerStepThrough]
    public static bool operator ==(LemmingSkill left, LemmingSkill right) => left.Id == right.Id;
    [DebuggerStepThrough]
    public static bool operator !=(LemmingSkill left, LemmingSkill right) => left.Id != right.Id;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LemmingSkillSet CreateBitArraySet(bool fullSet = false) => new(new LemmingSkillHasher(), fullSet);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<LemmingSkillHasher, LemmingSkillBitBuffer, LemmingSkill, TValue> CreateBitArrayDictionary<TValue>() => new(new LemmingSkillHasher());

    public readonly struct LemmingSkillHasher : IPerfectHasher<LemmingSkill>, IBitBufferCreator<LemmingSkillBitBuffer>
    {
        [Pure]
        public int NumberOfItems => EngineConstants.NumberOfLemmingSkills;
        [Pure]
        public int Hash(LemmingSkill item) => item.Id;
        [Pure]
        public LemmingSkill UnHash(int index) => LemmingSkills[index];

        public void CreateBitBuffer(out LemmingSkillBitBuffer buffer) => buffer = new();
    }

    [InlineArray(LemmingSkillBitBufferLength)]
    public struct LemmingSkillBitBuffer : IBitBuffer
    {
        private const int LemmingSkillBitBufferLength = (EngineConstants.NumberOfLemmingSkills + BitArrayHelpers.Mask) >>> BitArrayHelpers.Shift;

        private uint _0;

        public readonly int Length => LemmingSkillBitBufferLength;

        public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, LemmingSkillBitBufferLength);
        public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, LemmingSkillBitBufferLength);
    }
}
