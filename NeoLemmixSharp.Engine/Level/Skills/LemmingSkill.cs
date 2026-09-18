using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class LemmingSkill
{
    private const int NumberOfLemmingActionAndSkillPairs = LemmingActionConstants.NumberOfLemmingActions * LemmingSkillConstants.NumberOfLemmingSkills;

    private static readonly BitArraySet<LemmingActionAndSkillHasher, LemmingActionAndSkillPairBitBuffer, LemmingActionAndSkillPair> ActionsThatCanBeAssignedSkill = GetActionsThatCanBeAssignedSkill();

    private static readonly LemmingSkillSet ClassicSkills = GetClassicSkills();
    private static readonly LemmingSkillSet PermanentSkills = GetPermanentSkills();

    public static BitArrayEnumerable<LemmingSkillHasher, LemmingSkillType> AllClassicSkills => ClassicSkills.AsEnumerable();

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssignedPermanentSkill()
    {
        yield return LemmingActionType.AscenderAction;
        yield return LemmingActionType.BasherAction;
        yield return LemmingActionType.BlockerAction;
        yield return LemmingActionType.BuilderAction;
        yield return LemmingActionType.ClimberAction;
        yield return LemmingActionType.DehoisterAction;
        yield return LemmingActionType.DiggerAction;
        yield return LemmingActionType.DisarmerAction;
        yield return LemmingActionType.FallerAction;
        yield return LemmingActionType.FencerAction;
        yield return LemmingActionType.FloaterAction;
        yield return LemmingActionType.GliderAction;
        yield return LemmingActionType.HoisterAction;
        yield return LemmingActionType.JumperAction;
        yield return LemmingActionType.LasererAction;
        yield return LemmingActionType.MinerAction;
        yield return LemmingActionType.PlatformerAction;
        yield return LemmingActionType.ReacherAction;
        yield return LemmingActionType.RotateHalfAction;
        yield return LemmingActionType.RotateClockwiseAction;
        yield return LemmingActionType.RotateCounterclockwiseAction;
        yield return LemmingActionType.ShimmierAction;
        yield return LemmingActionType.ShruggerAction;
        yield return LemmingActionType.SliderAction;
        yield return LemmingActionType.StackerAction;
        yield return LemmingActionType.SwimmerAction;
        yield return LemmingActionType.WalkerAction;
    }

    public static IEnumerable<LemmingActionType> GetActionsThatCanBeAssignedRotationSkill()
    {
        yield return LemmingActionType.WalkerAction;
        yield return LemmingActionType.ShruggerAction;
        yield return LemmingActionType.PlatformerAction;
        yield return LemmingActionType.BuilderAction;
        yield return LemmingActionType.StackerAction;
        yield return LemmingActionType.BasherAction;
        yield return LemmingActionType.FencerAction;
        yield return LemmingActionType.MinerAction;
        yield return LemmingActionType.DiggerAction;
        yield return LemmingActionType.LasererAction;
    }

    private static BitArraySet<LemmingActionAndSkillHasher, LemmingActionAndSkillPairBitBuffer, LemmingActionAndSkillPair> GetActionsThatCanBeAssignedSkill()
    {
        var result = LemmingActionAndSkillHasher.CreateBitArraySet();

        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.ClimberSkill, ClimberSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.FloaterSkill, FloaterSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.BlockerSkill, BlockerSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.BomberSkill, BomberSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.BuilderSkill, BuilderSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.BasherSkill, BasherSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.MinerSkill, MinerSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.DiggerSkill, DiggerSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.WalkerSkill, WalkerSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.PlatformerSkill, PlatformerSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.StackerSkill, StackerSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.FencerSkill, FencerSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.GliderSkill, GliderSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.JumperSkill, JumperSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.SwimmerSkill, SwimmerSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.ShimmierSkill, ShimmierSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.LasererSkill, LasererSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.SliderSkill, SliderSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.DisarmerSkill, DisarmerSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.StonerSkill, StonerSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.ClonerSkill, ClonerSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.RotateClockwiseSkill, RotateClockwiseSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.RotateCounterclockwiseSkill, RotateCounterclockwiseSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.RotateHalfSkill, RotateHalfSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.AcidLemmingSkill, AcidLemmingSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.WaterLemmingSkill, WaterLemmingSkill.GetActionsThatCanBeAssigned());
        RegisterActionsThatCanBeAssignedSkill(LemmingSkillType.FastForwardSkill, FastForwardSkill.GetActionsThatCanBeAssigned());

        return result;

        void RegisterActionsThatCanBeAssignedSkill(LemmingSkillType skillType, IEnumerable<LemmingActionType> actionsThatCanBeAssignedSkill)
        {
            foreach (var actionType in actionsThatCanBeAssignedSkill)
            {
                var pair = new LemmingActionAndSkillPair(actionType, skillType);
                result.Add(pair);
            }
        }
    }

    private static LemmingSkillSet GetClassicSkills()
    {
        var result = LemmingSkillHasher.CreateBitArraySet();

        result.Add(LemmingSkillType.ClimberSkill);
        result.Add(LemmingSkillType.FloaterSkill);
        result.Add(LemmingSkillType.BomberSkill);
        result.Add(LemmingSkillType.BlockerSkill);
        result.Add(LemmingSkillType.BuilderSkill);
        result.Add(LemmingSkillType.BasherSkill);
        result.Add(LemmingSkillType.MinerSkill);
        result.Add(LemmingSkillType.DiggerSkill);

        return result;
    }

    private static LemmingSkillSet GetPermanentSkills()
    {
        var result = LemmingSkillHasher.CreateBitArraySet();

        result.Add(LemmingSkillType.ClimberSkill);
        result.Add(LemmingSkillType.FloaterSkill);
        result.Add(LemmingSkillType.GliderSkill);
        result.Add(LemmingSkillType.SliderSkill);
        result.Add(LemmingSkillType.SwimmerSkill);
        result.Add(LemmingSkillType.DisarmerSkill);
        result.Add(LemmingSkillType.AcidLemmingSkill);
        result.Add(LemmingSkillType.WaterLemmingSkill);
        result.Add(LemmingSkillType.FastForwardSkill);

        return result;
    }

    [Pure]
    public static bool IsClassicSkill(this LemmingSkillType lemmingSkillType) => ClassicSkills.Contains(lemmingSkillType);
    [Pure]
    public static bool IsPermanentSkill(this LemmingSkillType lemmingSkillType) => PermanentSkills.Contains(lemmingSkillType);

    public static bool CanAssignToLemming(Lemming lemming, LemmingSkillType skillType) => skillType switch
    {
        LemmingSkillType.ClimberSkill => ClimberSkill.CanAssignToLemming(lemming),
        LemmingSkillType.FloaterSkill => FloaterSkill.CanAssignToLemming(lemming),
        LemmingSkillType.BlockerSkill => BlockerSkill.CanAssignToLemming(lemming),
        LemmingSkillType.BomberSkill => BomberSkill.CanAssignToLemming(lemming),
        LemmingSkillType.BuilderSkill => BuilderSkill.CanAssignToLemming(lemming),
        LemmingSkillType.BasherSkill => BasherSkill.CanAssignToLemming(lemming),
        LemmingSkillType.MinerSkill => MinerSkill.CanAssignToLemming(lemming),
        LemmingSkillType.DiggerSkill => DiggerSkill.CanAssignToLemming(lemming),
        LemmingSkillType.WalkerSkill => WalkerSkill.CanAssignToLemming(lemming),
        LemmingSkillType.PlatformerSkill => PlatformerSkill.CanAssignToLemming(lemming),
        LemmingSkillType.StackerSkill => StackerSkill.CanAssignToLemming(lemming),
        LemmingSkillType.FencerSkill => FencerSkill.CanAssignToLemming(lemming),
        LemmingSkillType.GliderSkill => GliderSkill.CanAssignToLemming(lemming),
        LemmingSkillType.JumperSkill => JumperSkill.CanAssignToLemming(lemming),
        LemmingSkillType.SwimmerSkill => SwimmerSkill.CanAssignToLemming(lemming),
        LemmingSkillType.ShimmierSkill => ShimmierSkill.CanAssignToLemming(lemming),
        LemmingSkillType.LasererSkill => LasererSkill.CanAssignToLemming(lemming),
        LemmingSkillType.SliderSkill => SliderSkill.CanAssignToLemming(lemming),
        LemmingSkillType.DisarmerSkill => DisarmerSkill.CanAssignToLemming(lemming),
        LemmingSkillType.StonerSkill => StonerSkill.CanAssignToLemming(lemming),
        LemmingSkillType.ClonerSkill => ClonerSkill.CanAssignToLemming(lemming),
        LemmingSkillType.RotateClockwiseSkill => RotateClockwiseSkill.CanAssignToLemming(lemming),
        LemmingSkillType.RotateCounterclockwiseSkill => RotateCounterclockwiseSkill.CanAssignToLemming(lemming),
        LemmingSkillType.RotateHalfSkill => RotateHalfSkill.CanAssignToLemming(lemming),
        LemmingSkillType.AcidLemmingSkill => AcidLemmingSkill.CanAssignToLemming(lemming),
        LemmingSkillType.WaterLemmingSkill => WaterLemmingSkill.CanAssignToLemming(lemming),
        LemmingSkillType.FastForwardSkill => FastForwardSkill.CanAssignToLemming(lemming),

        _ => NoneSkill.CanAssignToLemming(lemming)
    };

    public static void AssignToLemming(Lemming lemming, LemmingSkillType skillType)
    {
        switch (skillType)
        {
            case LemmingSkillType.ClimberSkill: ClimberSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.FloaterSkill: FloaterSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.BlockerSkill: BlockerSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.BomberSkill: BomberSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.BuilderSkill: BuilderSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.BasherSkill: BasherSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.MinerSkill: MinerSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.DiggerSkill: DiggerSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.WalkerSkill: WalkerSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.PlatformerSkill: PlatformerSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.StackerSkill: StackerSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.FencerSkill: FencerSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.GliderSkill: GliderSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.JumperSkill: JumperSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.SwimmerSkill: SwimmerSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.ShimmierSkill: ShimmierSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.LasererSkill: LasererSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.SliderSkill: SliderSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.DisarmerSkill: DisarmerSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.StonerSkill: StonerSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.ClonerSkill: ClonerSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.RotateClockwiseSkill: RotateClockwiseSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.RotateCounterclockwiseSkill: RotateCounterclockwiseSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.RotateHalfSkill: RotateHalfSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.AcidLemmingSkill: AcidLemmingSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.WaterLemmingSkill: WaterLemmingSkill.AssignToLemming(lemming); break;
            case LemmingSkillType.FastForwardSkill: FastForwardSkill.AssignToLemming(lemming); break;

            default: NoneSkill.AssignToLemming(lemming); break;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SkillIsAssignableToCurrentAction(this LemmingSkillType skillType, LemmingActionType actionType)
    {
        var pair = new LemmingActionAndSkillPair(actionType, skillType);

        return ActionsThatCanBeAssignedSkill.Contains(pair);
    }

    private readonly record struct LemmingActionAndSkillPair(LemmingActionType ActionType, LemmingSkillType SkillType);

    private readonly struct LemmingActionAndSkillHasher : IBitBufferCreator<LemmingActionAndSkillPairBitBuffer, LemmingActionAndSkillPair>
    {
        [Pure]
        public int NumberOfItems => NumberOfLemmingActionAndSkillPairs;

        [Pure]
        public int Hash(LemmingActionAndSkillPair item)
        {
            var result = (int)item.ActionType;
            result *= LemmingSkillConstants.NumberOfLemmingSkills;
            result += (int)item.SkillType;
            return result;
        }

        [Pure]
        public LemmingActionAndSkillPair UnHash(int index)
        {
            var (actionTypeInt, skillTypeInt) = Math.DivRem((uint)index, LemmingSkillConstants.NumberOfLemmingSkills);

            return new LemmingActionAndSkillPair((LemmingActionType)actionTypeInt, (LemmingSkillType)skillTypeInt);
        }

        public void CreateBitBuffer(out LemmingActionAndSkillPairBitBuffer buffer) => buffer = new();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArraySet<LemmingActionAndSkillHasher, LemmingActionAndSkillPairBitBuffer, LemmingActionAndSkillPair> CreateBitArraySet() => new(new LemmingActionAndSkillHasher());
    }

    [InlineArray(LemmingActionAndSkillPairBitBufferLength)]
    private struct LemmingActionAndSkillPairBitBuffer : IBitBuffer
    {
        private const int LemmingActionAndSkillPairBitBufferLength = (NumberOfLemmingActionAndSkillPairs + BitArrayHelpers.Mask) >>> BitArrayHelpers.Shift;

        private uint _0;

        public readonly int Length => LemmingActionAndSkillPairBitBufferLength;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, LemmingActionAndSkillPairBitBufferLength);
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, LemmingActionAndSkillPairBitBufferLength);
    }
}
