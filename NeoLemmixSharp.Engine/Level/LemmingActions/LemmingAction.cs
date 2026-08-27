using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public static class LemmingAction
{
    [DebuggerDisplay("{ActionType}")]
    private readonly struct LemmingActionData(LemmingActionType actionType, byte numberOfAnimationFrames, byte maxPhysicsFrames, CursorSelectionPriority cursorSelectionPriority)
    {
        public readonly LemmingActionType ActionType = actionType;
        public readonly byte NumberOfAnimationFrames = numberOfAnimationFrames;
        public readonly byte MaxPhysicsFrames = maxPhysicsFrames;
        public readonly CursorSelectionPriority CursorSelectionPriority = cursorSelectionPriority;
    }

    private static readonly LemmingActionData[] LemmingActions = RegisterAllLemmingActions();
    private static readonly LemmingActionTypeSet AirborneActionTypes = GetAirborneActionTypes();
    private static readonly LemmingActionTypeSet OneTimeActionTypes = GetOneTimeActionTypes();

    private static LemmingActionData[] RegisterAllLemmingActions()
    {
        // NOTE: DO NOT ADD THE NONE ACTION
        var result = new LemmingActionData[]
        {
            new(LemmingActionType.WalkerAction,                 LemmingActionConstants.WalkerAnimationFrames,                 LemmingActionConstants.MaxWalkerPhysicsFrames,                 CursorSelectionPriority.WalkerMovementPriority),
            new(LemmingActionType.ClimberAction,                LemmingActionConstants.ClimberAnimationFrames,                LemmingActionConstants.MaxClimberPhysicsFrames,                CursorSelectionPriority.PermanentSkillPriority),
            new(LemmingActionType.FloaterAction,                LemmingActionConstants.FloaterAnimationFrames,                LemmingActionConstants.MaxFloaterPhysicsFrames,                CursorSelectionPriority.PermanentSkillPriority),
            new(LemmingActionType.BlockerAction,                LemmingActionConstants.BlockerAnimationFrames,                LemmingActionConstants.MaxBlockerPhysicsFrames,                CursorSelectionPriority.NonPermanentSkillPriority),
            new(LemmingActionType.BuilderAction,                LemmingActionConstants.BuilderAnimationFrames,                LemmingActionConstants.MaxBuilderPhysicsFrames,                CursorSelectionPriority.NonPermanentSkillPriority),
            new(LemmingActionType.BasherAction,                 LemmingActionConstants.BasherAnimationFrames,                 LemmingActionConstants.MaxBasherPhysicsFrames,                 CursorSelectionPriority.NonPermanentSkillPriority),
            new(LemmingActionType.MinerAction,                  LemmingActionConstants.MinerAnimationFrames,                  LemmingActionConstants.MaxMinerPhysicsFrames,                  CursorSelectionPriority.NonPermanentSkillPriority),
            new(LemmingActionType.DiggerAction,                 LemmingActionConstants.DiggerAnimationFrames,                 LemmingActionConstants.MaxDiggerPhysicsFrames,                 CursorSelectionPriority.NonPermanentSkillPriority),

            new(LemmingActionType.PlatformerAction,             LemmingActionConstants.PlatformerAnimationFrames,             LemmingActionConstants.MaxPlatformerPhysicsFrames,             CursorSelectionPriority.NonPermanentSkillPriority),
            new(LemmingActionType.StackerAction,                LemmingActionConstants.StackerAnimationFrames,                LemmingActionConstants.MaxStackerPhysicsFrames,                CursorSelectionPriority.NonPermanentSkillPriority),
            new(LemmingActionType.FencerAction,                 LemmingActionConstants.FencerAnimationFrames,                 LemmingActionConstants.MaxFencerPhysicsFrames,                 CursorSelectionPriority.NonPermanentSkillPriority),
            new(LemmingActionType.GliderAction,                 LemmingActionConstants.GliderAnimationFrames,                 LemmingActionConstants.MaxGliderPhysicsFrames,                 CursorSelectionPriority.PermanentSkillPriority),
            new(LemmingActionType.JumperAction,                 LemmingActionConstants.JumperAnimationFrames,                 LemmingActionConstants.MaxJumperPhysicsFrames,                 CursorSelectionPriority.NonWalkerMovementPriority),
            new(LemmingActionType.SwimmerAction,                LemmingActionConstants.SwimmerAnimationFrames,                LemmingActionConstants.MaxSwimmerPhysicsFrames,                CursorSelectionPriority.PermanentSkillPriority),
            new(LemmingActionType.ShimmierAction,               LemmingActionConstants.ShimmierAnimationFrames,               LemmingActionConstants.MaxShimmierPhysicsFrames,               CursorSelectionPriority.NonWalkerMovementPriority),
            new(LemmingActionType.LasererAction,                LemmingActionConstants.LasererAnimationFrames,                LemmingActionConstants.MaxLasererPhysicsFrames,                CursorSelectionPriority.NonPermanentSkillPriority),
            new(LemmingActionType.SliderAction,                 LemmingActionConstants.SliderAnimationFrames,                 LemmingActionConstants.MaxSliderPhysicsFrames,                 CursorSelectionPriority.PermanentSkillPriority),

            new(LemmingActionType.FallerAction,                 LemmingActionConstants.FallerAnimationFrames,                 LemmingActionConstants.MaxFallerPhysicsFrames,                 CursorSelectionPriority.NonWalkerMovementPriority),
            new(LemmingActionType.AscenderAction,               LemmingActionConstants.AscenderAnimationFrames,               LemmingActionConstants.MaxAscenderPhysicsFrames,               CursorSelectionPriority.WalkerMovementPriority),
            new(LemmingActionType.ShruggerAction,               LemmingActionConstants.ShruggerAnimationFrames,               LemmingActionConstants.MaxShruggerPhysicsFrames,               CursorSelectionPriority.NonWalkerMovementPriority),
            new(LemmingActionType.DrownerAction,                LemmingActionConstants.DrownerAnimationFrames,                LemmingActionConstants.MaxDrownerPhysicsFrames,                CursorSelectionPriority.NonWalkerMovementPriority),
            new(LemmingActionType.HoisterAction,                LemmingActionConstants.HoisterAnimationFrames,                LemmingActionConstants.MaxHoisterPhysicsFrames,                CursorSelectionPriority.NonWalkerMovementPriority),
            new(LemmingActionType.DehoisterAction,              LemmingActionConstants.DehoisterAnimationFrames,              LemmingActionConstants.MaxDehoisterPhysicsFrames,              CursorSelectionPriority.NonWalkerMovementPriority),
            new(LemmingActionType.ReacherAction,                LemmingActionConstants.ReacherAnimationFrames,                LemmingActionConstants.MaxReacherPhysicsFrames,                CursorSelectionPriority.NonWalkerMovementPriority),
            new(LemmingActionType.DisarmerAction,               LemmingActionConstants.DisarmerAnimationFrames,               LemmingActionConstants.MaxDisarmerPhysicsFrames,               CursorSelectionPriority.PermanentSkillPriority),

            new(LemmingActionType.ExiterAction,                 LemmingActionConstants.ExiterAnimationFrames,                 LemmingActionConstants.MaxExiterPhysicsFrames,                 CursorSelectionPriority.NoPriority),
            new(LemmingActionType.ExploderAction,               LemmingActionConstants.ExploderAnimationFrames,               LemmingActionConstants.MaxExploderPhysicsFrames,               CursorSelectionPriority.NoPriority),
            new(LemmingActionType.OhNoerAction,                 LemmingActionConstants.OhNoerAnimationFrames,                 LemmingActionConstants.MaxOhNoerPhysicsFrames,                 CursorSelectionPriority.NonWalkerMovementPriority),
            new(LemmingActionType.SplatterAction,               LemmingActionConstants.SplatterAnimationFrames,               LemmingActionConstants.MaxSplatterPhysicsFrames,               CursorSelectionPriority.NoPriority),
            new(LemmingActionType.StonerAction,                 LemmingActionConstants.StonerAnimationFrames,                 LemmingActionConstants.MaxStonerPhysicsFrames,                 CursorSelectionPriority.NoPriority),
            new(LemmingActionType.VaporiserAction,              LemmingActionConstants.VaporiserAnimationFrames,              LemmingActionConstants.MaxVaporizerPhysicsFrames,              CursorSelectionPriority.NoPriority),

            new(LemmingActionType.RotateClockwiseAction,        LemmingActionConstants.RotateClockwiseAnimationFrames,        LemmingActionConstants.MaxRotateClockwisePhysicsFrames,        CursorSelectionPriority.NonWalkerMovementPriority),
            new(LemmingActionType.RotateCounterclockwiseAction, LemmingActionConstants.RotateCounterclockwiseAnimationFrames, LemmingActionConstants.MaxRotateCounterclockwisePhysicsFrames, CursorSelectionPriority.NonWalkerMovementPriority),
            new(LemmingActionType.RotateHalfAction,             LemmingActionConstants.RotateHalfAnimationFrames,             LemmingActionConstants.MaxRotateHalfPhysicsFrames,             CursorSelectionPriority.NonWalkerMovementPriority),
        };

        Debug.Assert(result.Length == LemmingActionConstants.NumberOfLemmingActions);

        var hasher = new LemmingActionTypeHasher();
        hasher.AssertUniqueIds(new ReadOnlySpan<LemmingActionData>(result));
        Array.Sort(result, hasher);

        return result;
    }

    private static LemmingActionTypeSet GetAirborneActionTypes()
    {
        var result = CreateBitArraySet();

        result.Add(LemmingActionType.DrownerAction);
        result.Add(LemmingActionType.FallerAction);
        result.Add(LemmingActionType.FloaterAction);
        result.Add(LemmingActionType.GliderAction);
        result.Add(LemmingActionType.JumperAction);
        result.Add(LemmingActionType.ReacherAction);
        result.Add(LemmingActionType.RotateClockwiseAction);
        result.Add(LemmingActionType.RotateCounterclockwiseAction);
        result.Add(LemmingActionType.RotateHalfAction);
        result.Add(LemmingActionType.ShimmierAction);
        result.Add(LemmingActionType.SwimmerAction);
        result.Add(LemmingActionType.VaporiserAction);

        return result;
    }

    private static LemmingActionTypeSet GetOneTimeActionTypes()
    {
        var result = CreateBitArraySet();

        result.Add(LemmingActionType.DehoisterAction);
        result.Add(LemmingActionType.DrownerAction);
        result.Add(LemmingActionType.ExiterAction);
        result.Add(LemmingActionType.ExploderAction);
        result.Add(LemmingActionType.HoisterAction);
        result.Add(LemmingActionType.OhNoerAction);
        result.Add(LemmingActionType.ReacherAction);
        result.Add(LemmingActionType.RotateClockwiseAction);
        result.Add(LemmingActionType.RotateCounterclockwiseAction);
        result.Add(LemmingActionType.RotateHalfAction);
        result.Add(LemmingActionType.ShruggerAction);
        result.Add(LemmingActionType.SplatterAction);
        result.Add(LemmingActionType.StonerAction);
        result.Add(LemmingActionType.VaporiserAction);

        return result;
    }

    public static bool IsAirborneAction(LemmingActionType actionType) => AirborneActionTypes.Contains(actionType);
    public static bool IsOneTimeAction(LemmingActionType actionType) => OneTimeActionTypes.Contains(actionType);

    public static int GetNumberOfAnimationFramesForActionType(LemmingActionType actionType)
    {
        int result = 1;

        if ((uint)actionType < LemmingActionConstants.NumberOfLemmingActions)
            result = LemmingActions.At((int)actionType).NumberOfAnimationFrames;

        return result;
    }

    public static int GetMaxPhysicsFramesForActionType(LemmingActionType actionType)
    {
        int result = 1;

        if ((uint)actionType < LemmingActionConstants.NumberOfLemmingActions)
            result = LemmingActions.At((int)actionType).MaxPhysicsFrames;

        return result;
    }

    public static CursorSelectionPriority GetCursorSelectionPriorityForActionType(LemmingActionType actionType)
    {
        var result = CursorSelectionPriority.NoneActionPriority;

        if ((uint)actionType < LemmingActionConstants.NumberOfLemmingActions)
            result = LemmingActions.At((int)actionType).CursorSelectionPriority;

        return result;
    }

    public static bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming, LemmingActionType actionType) => actionType switch
    {
        LemmingActionType.WalkerAction => WalkerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.ClimberAction => ClimberAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.FloaterAction => FloaterAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.BlockerAction => BlockerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.BuilderAction => BuilderAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.BasherAction => BasherAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.MinerAction => MinerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.DiggerAction => DiggerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.PlatformerAction => PlatformerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.StackerAction => StackerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.FencerAction => FencerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.GliderAction => GliderAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.JumperAction => JumperAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.SwimmerAction => SwimmerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.ShimmierAction => ShimmierAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.LasererAction => LasererAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.SliderAction => SliderAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.FallerAction => FallerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.AscenderAction => AscenderAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.ShruggerAction => ShruggerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.DrownerAction => DrownerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.HoisterAction => HoisterAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.DehoisterAction => DehoisterAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.ReacherAction => ReacherAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.DisarmerAction => DisarmerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.ExiterAction => ExiterAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.ExploderAction => ExploderAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.OhNoerAction => OhNoerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.SplatterAction => SplatterAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.StonerAction => StonerAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.VaporiserAction => VaporiserAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.RotateClockwiseAction => RotateClockwiseAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.RotateCounterclockwiseAction => RotateCounterclockwiseAction.UpdateLemming(lemming, in gadgetsNearLemming),
        LemmingActionType.RotateHalfAction => RotateHalfAction.UpdateLemming(lemming, in gadgetsNearLemming),

        _ => NoneAction.UpdateLemming(lemming, in gadgetsNearLemming),
    };

    public static void TransitionLemmingToAction(Lemming lemming, bool turnAround, LemmingActionType actionType)
    {
        switch (actionType)
        {
            case LemmingActionType.WalkerAction: WalkerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.ClimberAction: ClimberAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.FloaterAction: FloaterAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.BlockerAction: BlockerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.BuilderAction: BuilderAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.BasherAction: BasherAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.MinerAction: MinerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.DiggerAction: DiggerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.PlatformerAction: PlatformerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.StackerAction: StackerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.FencerAction: FencerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.GliderAction: GliderAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.JumperAction: JumperAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.SwimmerAction: SwimmerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.ShimmierAction: ShimmierAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.LasererAction: LasererAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.SliderAction: SliderAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.FallerAction: FallerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.AscenderAction: AscenderAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.ShruggerAction: ShruggerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.DrownerAction: DrownerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.HoisterAction: HoisterAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.DehoisterAction: DehoisterAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.ReacherAction: ReacherAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.DisarmerAction: DisarmerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.ExiterAction: ExiterAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.ExploderAction: ExploderAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.OhNoerAction: OhNoerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.SplatterAction: SplatterAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.StonerAction: StonerAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.VaporiserAction: VaporiserAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.RotateClockwiseAction: RotateClockwiseAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.RotateCounterclockwiseAction: RotateCounterclockwiseAction.TransitionLemmingToAction(lemming, turnAround); break;
            case LemmingActionType.RotateHalfAction: RotateHalfAction.TransitionLemmingToAction(lemming, turnAround); break;

            default: NoneAction.TransitionLemmingToAction(lemming, turnAround); break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DoMainTransitionActions(
        this LemmingActionType actionType,
        Lemming lemming,
        bool turnAround)
    {
        if (lemming.CurrentActionType == LemmingActionType.BlockerAction &&
            actionType != LemmingActionType.BlockerAction &&
            actionType != LemmingActionType.OhNoerAction)
        {
            // Need to de-register blocker from LemmingManager
            // when transitioning from a blocker. Exceptions are for
            // transitions to blocker or ohNoer

            LevelScreen.LemmingManager.DeregisterBlocker(lemming);
        }

        var turnAroundXor = turnAround ? 1 : 0;
        turnAroundXor ^= lemming.FacingDirection.Id;
        lemming.FacingDirection = new FacingDirection(turnAroundXor);

        if (actionType == lemming.CurrentActionType)
            return;

        lemming.SetCurrentActionType(actionType);
        lemming.PhysicsFrame = 0;
        lemming.AnimationFrame = 0;
        lemming.EndOfAnimation = false;
        lemming.NumberOfBricksLeft = 0;
        lemming.IsStartingAction = true;
        lemming.InitialFall = false;
    }

    public static RectangularRegion GetLemmingBounds(this LemmingActionType actionType, Lemming lemming)
    {
        var dht = lemming.DihedralTransformation;
        var actionBounds = LemmingActionBounds.GetBounds(actionType);

        actionBounds = dht.Transform(actionBounds);
        actionBounds = actionBounds.Translate(lemming.AnchorPosition);

        return actionBounds;
    }

    public static Point GetFootPosition(this LemmingActionType actionType, DihedralTransformation dht, Point anchorPosition)
    {
        if (actionType is LemmingActionType.ClimberAction or LemmingActionType.SliderAction)
            return dht.Orientation.MoveLeft(anchorPosition, dht.FacingDirection.DeltaX);

        return dht.Orientation.MoveUp(anchorPosition, 1);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LemmingActionTypeSet CreateBitArraySet() => new(new LemmingActionTypeHasher());

    public readonly struct LemmingActionTypeHasher : IBitBufferCreator<LemmingActionBitBuffer, LemmingActionData>, IBitBufferCreator<LemmingActionBitBuffer, LemmingActionType>
    {
        [Pure]
        public int NumberOfItems => LemmingActionConstants.NumberOfLemmingActions;
        [Pure]
        int IPerfectHasher<LemmingActionType>.Hash(LemmingActionType item) => (int)item;
        [Pure]
        LemmingActionType IPerfectHasher<LemmingActionType>.UnHash(int index) => (LemmingActionType)index;
        [Pure]
        int IPerfectHasher<LemmingActionData>.Hash(LemmingActionData item) => (int)item.ActionType;
        [Pure]
        LemmingActionData IPerfectHasher<LemmingActionData>.UnHash(int index) => LemmingActions.At(index);

        public void CreateBitBuffer(out LemmingActionBitBuffer buffer) => buffer = new();
    }

    [InlineArray(LemmingActionBitBufferLength)]
    public struct LemmingActionBitBuffer : IBitBuffer
    {
        private const int LemmingActionBitBufferLength = (LemmingActionConstants.NumberOfLemmingActions + BitArrayHelpers.Mask) >>> BitArrayHelpers.Shift;

        private uint _0;

        public readonly int Length => LemmingActionBitBufferLength;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, LemmingActionBitBufferLength);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, LemmingActionBitBufferLength);
    }
}
