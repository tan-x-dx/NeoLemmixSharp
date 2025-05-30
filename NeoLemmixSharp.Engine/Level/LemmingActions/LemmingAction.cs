using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public abstract class LemmingAction : IIdEquatable<LemmingAction>
{
    private static readonly LemmingAction[] LemmingActions = RegisterAllLemmingActions();
    private static readonly LemmingActionSet AirborneActions = GetAirborneActions();
    private static readonly LemmingActionSet OneTimeActions = GetOneTimeActions();

    public const int NumberOfItems = LemmingActionConstants.NumberOfLemmingActions;
    public static ReadOnlySpan<LemmingAction> AllItems => new(LemmingActions);

    private static LemmingAction[] RegisterAllLemmingActions()
    {
        // NOTE: DO NOT ADD THE NONE ACTION
        var result = new LemmingAction[]
        {
            WalkerAction.Instance,
            ClimberAction.Instance,
            FloaterAction.Instance,
            BlockerAction.Instance,
            BuilderAction.Instance,
            BasherAction.Instance,
            MinerAction.Instance,
            DiggerAction.Instance,

            PlatformerAction.Instance,
            StackerAction.Instance,
            FencerAction.Instance,
            GliderAction.Instance,
            JumperAction.Instance,
            SwimmerAction.Instance,
            ShimmierAction.Instance,
            LasererAction.Instance,
            SliderAction.Instance,

            FallerAction.Instance,
            AscenderAction.Instance,
            ShruggerAction.Instance,
            DrownerAction.Instance,
            HoisterAction.Instance,
            DehoisterAction.Instance,
            ReacherAction.Instance,
            DisarmerAction.Instance,

            ExiterAction.Instance,
            ExploderAction.Instance,
            OhNoerAction.Instance,
            SplatterAction.Instance,
            StonerAction.Instance,
            VaporiserAction.Instance,

            RotateClockwiseAction.Instance,
            RotateCounterclockwiseAction.Instance,
            RotateHalfAction.Instance
        };

        Debug.Assert(result.Length == LemmingActionConstants.NumberOfLemmingActions);

        var hasher = new LemmingActionHasher();
        hasher.AssertUniqueIds(new ReadOnlySpan<LemmingAction>(result));
        Array.Sort(result, hasher);

        return result;
    }

    private static LemmingActionSet GetAirborneActions()
    {
        var result = CreateBitArraySet();

        result.Add(DrownerAction.Instance);
        result.Add(FallerAction.Instance);
        result.Add(FloaterAction.Instance);
        result.Add(GliderAction.Instance);
        result.Add(JumperAction.Instance);
        result.Add(ReacherAction.Instance);
        result.Add(RotateClockwiseAction.Instance);
        result.Add(RotateCounterclockwiseAction.Instance);
        result.Add(RotateHalfAction.Instance);
        result.Add(ShimmierAction.Instance);
        result.Add(SwimmerAction.Instance);
        result.Add(VaporiserAction.Instance);

        return result;
    }

    private static LemmingActionSet GetOneTimeActions()
    {
        var result = CreateBitArraySet();

        result.Add(DehoisterAction.Instance);
        result.Add(DrownerAction.Instance);
        result.Add(ExiterAction.Instance);
        result.Add(ExploderAction.Instance);
        result.Add(HoisterAction.Instance);
        result.Add(OhNoerAction.Instance);
        result.Add(ReacherAction.Instance);
        result.Add(RotateClockwiseAction.Instance);
        result.Add(RotateCounterclockwiseAction.Instance);
        result.Add(RotateHalfAction.Instance);
        result.Add(ShruggerAction.Instance);
        result.Add(SplatterAction.Instance);
        result.Add(StonerAction.Instance);
        result.Add(VaporiserAction.Instance);

        return result;
    }

    /// <summary>
    /// Safe alternative to performing the array lookup - the input may be negative, or an invalid id. In such a case the <see cref="NoneAction"/> is returned
    /// </summary>
    /// <param name="unboundActionId">The (possibly invalid) id of the action to fetch.</param>
    /// <returns>The LemmingAction with that id, or the <see cref="NoneAction"/> if the id is invalid.</returns>
    public static LemmingAction GetActionOrDefault(int unboundActionId)
    {
        return (uint)unboundActionId < (uint)LemmingActions.Length
            ? LemmingActions[unboundActionId]
            : NoneAction.Instance;
    }

    public readonly int Id;
    public readonly string LemmingActionName;
    public readonly string LemmingActionSpriteFileName;
    public readonly int NumberOfAnimationFrames;
    public readonly int MaxPhysicsFrames;
    public readonly int CursorSelectionPriorityValue;

    protected LemmingAction(
        int id,
        string lemmingActionName,
        string lemmingActionSpriteFileName,
        int numberOfAnimationFrames,
        int maxPhysicsFrames,
        int cursorSelectionPriorityValue)
    {
        Id = id;
        LemmingActionName = lemmingActionName;
        LemmingActionSpriteFileName = lemmingActionSpriteFileName;
        NumberOfAnimationFrames = numberOfAnimationFrames;
        MaxPhysicsFrames = maxPhysicsFrames;
        CursorSelectionPriorityValue = cursorSelectionPriorityValue;
    }

    public abstract bool UpdateLemming(Lemming lemming, in GadgetEnumerable gadgetsNearLemming);

    public RectangularRegion GetLemmingBounds(Lemming lemming)
    {
        var dht = new DihedralTransformation(lemming.Orientation, lemming.FacingDirection);
        var actionBounds = ActionBounds();

        actionBounds = dht.Transform(actionBounds);
        actionBounds = actionBounds.Translate(lemming.AnchorPosition);

        return actionBounds;
    }

    protected virtual RectangularRegion ActionBounds() => LemmingActionBounds.StandardLemmingBounds;

    public virtual Point GetFootPosition(
        Lemming lemming,
        Point anchorPosition)
    {
        return lemming.Orientation.MoveUp(anchorPosition, 1);
    }

    public abstract void TransitionLemmingToAction(
        Lemming lemming,
        bool turnAround);

    protected void DoMainTransitionActions(
        Lemming lemming,
        bool turnAround)
    {
        if (lemming.CurrentAction.Id == LemmingActionConstants.BlockerActionId &&
            Id != LemmingActionConstants.BlockerActionId &&
            Id != LemmingActionConstants.OhNoerActionId)
        {
            // Need to de-register blocker from LemmingManager
            // when transitioning from a blocker. Exceptions are for
            // transitions to blocker or ohNoer

            LevelScreen.LemmingManager.DeregisterBlocker(lemming);
        }

        if (turnAround)
        {
            lemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());
        }

        if (lemming.CurrentAction == this)
            return;

        lemming.SetCurrentAction(this);
        lemming.PhysicsFrame = 0;
        lemming.AnimationFrame = 0;
        lemming.EndOfAnimation = false;
        lemming.NumberOfBricksLeft = 0;
        lemming.IsStartingAction = true;
        lemming.InitialFall = false;
    }

    public bool IsAirborneAction() => AirborneActions.Contains(this);
    public bool IsOneTimeAction() => OneTimeActions.Contains(this);

    int IIdEquatable<LemmingAction>.Id => Id;
    [DebuggerStepThrough]
    public bool Equals(LemmingAction? other) => Id == (other?.Id ?? -1);
    [DebuggerStepThrough]
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is LemmingAction other && Id == other.Id;
    [DebuggerStepThrough]
    public sealed override int GetHashCode() => Id;
    [DebuggerStepThrough]
    public sealed override string ToString() => LemmingActionName;

    [DebuggerStepThrough]
    public static bool operator ==(LemmingAction left, LemmingAction right) => left.Id == right.Id;
    [DebuggerStepThrough]
    public static bool operator !=(LemmingAction left, LemmingAction right) => left.Id != right.Id;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LemmingActionSet CreateBitArraySet() => new(new LemmingActionHasher());
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<LemmingActionHasher, LemmingActionBitBuffer, LemmingAction, TValue> CreateBitArrayDictionary<TValue>() => new(new LemmingActionHasher());

    public readonly struct LemmingActionHasher : IPerfectHasher<LemmingAction>, IBitBufferCreator<LemmingActionBitBuffer>
    {
        [Pure]
        public int NumberOfItems => LemmingActionConstants.NumberOfLemmingActions;
        [Pure]
        public int Hash(LemmingAction item) => item.Id;
        [Pure]
        public LemmingAction UnHash(int index) => LemmingActions[index];

        public void CreateBitBuffer(out LemmingActionBitBuffer buffer) => buffer = new();
    }

    [InlineArray(LemmingActionBitBufferLength)]
    public struct LemmingActionBitBuffer : IBitBuffer
    {
        private const int LemmingActionBitBufferLength = (LemmingActionConstants.NumberOfLemmingActions + BitArrayHelpers.Mask) >>> BitArrayHelpers.Shift;

        private uint _0;

        public readonly int Length => LemmingActionBitBufferLength;

        public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, LemmingActionBitBufferLength);
        public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, LemmingActionBitBufferLength);
    }
}