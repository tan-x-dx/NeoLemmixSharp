using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public abstract class LemmingAction : IExtendedEnumType<LemmingAction>
{
    private static readonly LemmingAction[] LemmingActions = RegisterAllLemmingActions();
    private static readonly LemmingActionSet AirborneActions = GetAirborneActions();
    private static readonly LemmingActionSet OneTimeActions = GetOneTimeActions();

    public static int NumberOfItems => LemmingActions.Length;
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

        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<LemmingAction>(result));
        Array.Sort(result, IdEquatableItemHelperMethods.Compare);

        return result;
    }

    private static LemmingActionSet GetAirborneActions()
    {
        var result = CreateEmptySimpleSet();

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
        var result = CreateEmptySimpleSet();

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
    /// Safe alternative to performing the array lookup - the input may be negative, or an invalid id. In such a case - the NoneAction is returned.
    /// </summary>
    /// <param name="unboundActionId">The (possibly invalid) id of the action to fetch.</param>
    /// <returns>The LemmingAction with that id, or the NoneAction if the id is invalid.</returns>
    public static LemmingAction GetActionFromUnboundId(int unboundActionId)
    {
        return (uint)unboundActionId >= (uint)LemmingActions.Length
            ? NoneAction.Instance
            : LemmingActions[unboundActionId];
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LemmingActionSet CreateEmptySimpleSet() => ExtendedEnumTypeComparer<LemmingAction>.CreateSimpleSet();

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

    public abstract bool UpdateLemming(Lemming lemming);

    public LevelRegion GetLemmingBounds(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dxCorrection = 1 - lemming.FacingDirection.Id; // Fixes off-by-one errors with left/right positions
        var lemmingPosition = lemming.LevelPosition;
        var physicsFrame = lemming.PhysicsFrame;

        var topLeftDx = TopLeftBoundsDeltaX(physicsFrame);
        var topLeftDy = TopLeftBoundsDeltaY(physicsFrame);

        var bottomRightDx = BottomRightBoundsDeltaX(physicsFrame);
        var bottomRightDy = BottomRightBoundsDeltaY(physicsFrame);

        var p1 = orientation.MoveWithoutNormalization(lemmingPosition, dxCorrection + dx * topLeftDx, topLeftDy);
        var p2 = orientation.MoveWithoutNormalization(lemmingPosition, dxCorrection + dx * bottomRightDx, bottomRightDy);

        return new LevelRegion(p1, p2);
    }

    protected abstract int TopLeftBoundsDeltaX(int animationFrame);
    protected abstract int TopLeftBoundsDeltaY(int animationFrame);

    protected abstract int BottomRightBoundsDeltaX(int animationFrame);
    protected virtual int BottomRightBoundsDeltaY(int animationFrame) => -1;

    public virtual LevelPosition GetFootPosition(
        Lemming lemming,
        LevelPosition anchorPosition)
    {
        return lemming.Orientation.MoveUp(anchorPosition, 1);
    }

    public virtual void TransitionLemmingToAction(
        Lemming lemming,
        bool turnAround)
    {
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
    public bool Equals(LemmingAction? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is LemmingAction other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => LemmingActionName;

    public static bool operator ==(LemmingAction left, LemmingAction right) => left.Id == right.Id;
    public static bool operator !=(LemmingAction left, LemmingAction right) => left.Id != right.Id;
}