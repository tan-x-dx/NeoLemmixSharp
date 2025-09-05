using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class Lemming : IIdEquatable<Lemming>, IRectangularBounds, ISnapshotDataConvertible
{
    public static Lemming SimulationLemming { get; } = new();

    public readonly int Id;

    public LemmingData Data;

    RectangularRegion IRectangularBounds.CurrentBounds => Data.CurrentBounds;

    public LemmingState State { get; }

    public LemmingAction PreviousAction { get; private set; } = NoneAction.Instance;
    public LemmingAction CurrentAction { get; private set; }
    public LemmingAction NextAction { get; private set; } = NoneAction.Instance;
    public LemmingAction CountDownAction { get; private set; } = NoneAction.Instance;

    public LemmingRenderer Renderer { get; }

    public bool IsSimulation => Id < 0;
    public bool IsFastForward => Data.FastForwardTime > 0 || State.IsPermanentFastForwards;

    public Point HeadPosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Data.Orientation.MoveUp(Data.AnchorPosition, 6);
    }

    public Point FootPosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => CurrentAction.GetFootPosition(this, Data.AnchorPosition);
    }

    public Point CenterPosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Data.Orientation.MoveUp(Data.AnchorPosition, 4);
    }

    public Lemming(
        int id,
        Orientation orientation,
        FacingDirection facingDirection,
        int initialActionId,
        int tribeId)
    {
        Id = id;
        Data.Orientation = orientation;
        Data.FacingDirection = facingDirection;
        CurrentAction = LemmingAction.GetActionOrDefault(initialActionId);
        State = new LemmingState(this, tribeId);
        Renderer = new LemmingRenderer(this);
    }

    private Lemming()
    {
        Id = -1;
        Data.Orientation = Orientation.Down;
        Data.FacingDirection = FacingDirection.Right;
        CurrentAction = NoneAction.Instance;
        State = new LemmingState(this, EngineConstants.ClassicTribeId);
        Renderer = new LemmingRenderer(this);
    }

    public void Initialise()
    {
        State.IsActive = true;
        Data.PreviousAnchorPosition = Data.AnchorPosition;
        Data.CurrentBounds = CurrentAction.GetLemmingBounds(this);

        var initialAction = CurrentAction;
        if (initialAction == NoneAction.Instance)
        {
            initialAction = WalkerAction.Instance;
        }

        initialAction.TransitionLemmingToAction(this, false);
        Renderer.UpdateLemmingState(true);
    }

    [SkipLocalsInit]
    public void Tick()
    {
        PreviousAction = CurrentAction;
        // No transition to do at the end of lemming movement
        NextAction = NoneAction.Instance;

        HandleParticleTimer();
        HandleCountDownTimer();
        HandleFastForwardTimer();

        Span<Point> gadgetCheckPositions = stackalloc Point[LemmingMovementHelper.MaxIntermediateCheckPositions];

        // Use first four entries of span to hold level positions.
        // To do gadget checks, fetch all gadgets that overlap a certain rectangle.
        // That rectangle is defined as being the minimum bounding box of four level positions:
        // the anchor and foot positions of the previous frame, and a large box around the current position.
        // Fixes (literal) edge cases when lemmings and gadgets pass chunk position boundaries
        var p = Data.Orientation.Move(Data.AnchorPosition, -5, -12);
        gadgetCheckPositions[0] = p;
        p = Data.Orientation.Move(Data.AnchorPosition, 5, 12);
        gadgetCheckPositions[1] = p;
        p = Data.PreviousAnchorPosition;
        gadgetCheckPositions[2] = p;
        p = PreviousAction.GetFootPosition(this, p);
        gadgetCheckPositions[3] = p;

        var checkPositionsBounds = new RectangularRegion(gadgetCheckPositions[..4]);

        Span<uint> scratchSpaceSpan = stackalloc uint[LevelScreen.GadgetManager.ScratchSpaceSize];
        LevelScreen.GadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, checkPositionsBounds, out var gadgetsNearLemming);

        var checkZombies = HandleLemmingAction(in gadgetsNearLemming) &&
                           CheckLevelBoundaries() &&
                           CheckTriggerAreas(false, gadgetCheckPositions, in gadgetsNearLemming) &&
                           CurrentAction != ExiterAction.Instance &&
                           !State.IsZombie &&
                           LevelScreen.LemmingManager.AnyZombies();

        if (checkZombies)
        {
            LevelScreen.LemmingManager.DoZombieCheck(this);
        }
    }

    [SkipLocalsInit]
    public void Simulate(bool checkGadgets)
    {
        if (!IsSimulation)
            throw new InvalidOperationException("Use simulation lemming for simulations!");

        HandleParticleTimer();
        HandleCountDownTimer();
        HandleFastForwardTimer();

        Span<Point> gadgetCheckPositions = stackalloc Point[LemmingMovementHelper.MaxIntermediateCheckPositions];

        // Use first four entries of span to hold level positions.
        // To do gadget checks, fetch all gadgets that overlap a certain rectangle.
        // That rectangle is defined as being the minimum bounding box of four level positions:
        // the anchor and foot positions of the previous frame, and a large box around the current position.
        // Fixes (literal) edge cases when lemmings and gadgets pass chunk position boundaries
        gadgetCheckPositions[0] = Data.Orientation.Move(Data.AnchorPosition, -8, -16);
        gadgetCheckPositions[1] = Data.Orientation.Move(Data.AnchorPosition, 8, 16);
        gadgetCheckPositions[2] = Data.PreviousAnchorPosition;
        gadgetCheckPositions[3] = PreviousAction.GetFootPosition(this, gadgetCheckPositions[2]);

        var checkPositionsBounds = new RectangularRegion(gadgetCheckPositions[..4]);

        Span<uint> scratchSpaceSpan = stackalloc uint[LevelScreen.GadgetManager.ScratchSpaceSize];
        LevelScreen.GadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, checkPositionsBounds, out var gadgetsNearLemming);

        var handleGadgets = HandleLemmingAction(in gadgetsNearLemming) && CheckLevelBoundaries() && checkGadgets;
        if (handleGadgets)
        {
            // Reuse the above span. LemmingMovementHelper will overwrite existing values
            CheckTriggerAreas(false, gadgetCheckPositions, in gadgetsNearLemming);
        }
    }

    private void HandleParticleTimer()
    {
        if (Data.ParticleTimer > 0)
        {
            Data.ParticleTimer--;
        }
    }

    private void HandleCountDownTimer()
    {
        if (Data.CountDownTimer == 0)
            return;

        Data.CountDownTimer--;
        CountDownHelper.UpdateCountDownTimer(this);

        if (Data.CountDownTimer != 0)
            return;

        OhNoerAction.HandleCountDownTransition(this);
    }

    private void HandleFastForwardTimer()
    {
        if (Data.FastForwardTime > 0)
        {
            Data.FastForwardTime--;
        }
        else
        {
            LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
        }
    }

    /// <summary>
    /// Handle one frame of the lemming's action, and update data accordingly.
    /// Some actions (e.g. exiter, splatter, etc) may result in the lemming being killed off in some way.
    /// In that case, the method returns false - no extra work is necessary (don't bother checking gadgets, for example).
    /// </summary>
    /// <returns>True if more work needs to be done this frame</returns>
    private bool HandleLemmingAction(in GadgetEnumerable gadgetsNearLemming)
    {
        var frame = Data.AnimationFrame + 1;
        if (frame == CurrentAction.NumberOfAnimationFrames)
        {
            // Floater and Glider start cycle at frame 9!
            if (CurrentAction == FloaterAction.Instance ||
                CurrentAction == GliderAction.Instance)
            {
                frame = EngineConstants.FloaterGliderStartCycleFrame;
            }
            else
            {
                frame = 0;
            }
        }
        Data.AnimationFrame = frame;

        frame = Data.PhysicsFrame + 1;
        if (frame == CurrentAction.MaxPhysicsFrames)
        {
            // Floater and Glider start cycle at frame 9!
            if (CurrentAction == FloaterAction.Instance ||
                CurrentAction == GliderAction.Instance)
            {
                frame = EngineConstants.FloaterGliderStartCycleFrame;
            }
            else
            {
                frame = 0;
            }

            Data.EndOfAnimation = CurrentAction.IsOneTimeAction();
        }

        Data.PhysicsFrame = frame;
        Data.PreviousAnchorPosition = Data.AnchorPosition;

        var result = CurrentAction.UpdateLemming(this, in gadgetsNearLemming);
        Data.CurrentBounds = CurrentAction.GetLemmingBounds(this);

        return result;
    }

    private bool CheckLevelBoundaries()
    {
        var terrainManager = LevelScreen.TerrainManager;
        var footPixel = terrainManager.PixelTypeAtPosition(FootPosition);
        var headPixel = terrainManager.PixelTypeAtPosition(HeadPosition);

        if (!footPixel.IsVoid() || !headPixel.IsVoid())
            return true;

        LevelScreen.LemmingManager.RemoveLemming(this, LemmingRemovalReason.DeathVoid);
        return false;
    }

    private bool CheckTriggerAreas(
        bool isPostTeleportCheck,
        Span<Point> gadgetCheckPositions,
        in GadgetEnumerable gadgetsNearLemming)
    {
        if (isPostTeleportCheck)
        {
            Data.PreviousAnchorPosition = Data.AnchorPosition;
        }

        var result = CheckGadgets(gadgetCheckPositions, in gadgetsNearLemming) && LemmingManager.DoBlockerCheck(this);

        NextAction.TransitionLemmingToAction(this, false);

        return result;
    }

    private bool CheckGadgets(
        Span<Point> gadgetCheckPositions,
        in GadgetEnumerable gadgetsNearLemming)
    {
        if (gadgetsNearLemming.Count == 0)
            return true;

        var movementHelper = new LemmingMovementHelper(this, gadgetCheckPositions);
        var length = movementHelper.EvaluateCheckPositions();

        return CheckGadgetHitBoxCollisions(in gadgetsNearLemming, gadgetCheckPositions[..length]);
    }

    private bool CheckGadgetHitBoxCollisions(in GadgetEnumerable gadgetEnumerable, ReadOnlySpan<Point> intermediatePositions)
    {
        foreach (var gadget in gadgetEnumerable)
        {
            var currentState = gadget.CurrentState;

            foreach (var anchorPosition in intermediatePositions)
            {
                var footPosition = CurrentAction.GetFootPosition(this, anchorPosition);
                if (!gadget.ContainsEitherPoint(Data.Orientation, anchorPosition, footPosition))
                    continue;

                var filters = currentState.Filters;
                LemmingHitBoxFilter? firstMatchingFilter = null;

                for (var i = 0; i < filters.Length; i++)
                {
                    var filter = filters[i];

                    if (filter.MatchesLemming(this))
                    {
                        firstMatchingFilter = filter;
                        break;
                    }
                }

                if (firstMatchingFilter is null)
                    continue;

                var beforeAction = CurrentAction;
                HandleGadgetInteraction(gadget, firstMatchingFilter, anchorPosition);
                var afterAction = CurrentAction;

                if (beforeAction == afterAction)
                    continue;

                Data.AnchorPosition = anchorPosition;

                Data.CurrentBounds = afterAction.GetLemmingBounds(this);

                return false;
            }
        }

        return true;
    }

    private void HandleGadgetInteraction(
        HitBoxGadget gadget,
        LemmingHitBoxFilter filter,
        Point checkPosition)
    {
        // If we're at the end of the check positions and Next action is not None
        // then transition. However, if NextAction is SplatterAction and there's water
        // at the position, the water takes precedence over splatting
        if (NextAction != NoneAction.Instance &&
            checkPosition == Data.AnchorPosition &&
            (NextAction != SplatterAction.Instance ||
            filter.HitBoxBehaviour != HitBoxInteractionType.Liquid))
        {
            NextAction.TransitionLemmingToAction(this, false);
            if (Data.JumpToHoistAdvance)
            {
                Data.AnimationFrame += 2;
                Data.PhysicsFrame += 2;
                Data.JumpToHoistAdvance = false;
            }

            NextAction = NoneAction.Instance;
        }

        gadget.OnLemmingHit(filter, this);
    }

    public void SetFacingDirection(FacingDirection newFacingDirection)
    {
        Data.FacingDirection = newFacingDirection;
        Renderer.UpdateLemmingState(true);
    }

    public void SetOrientation(Orientation newOrientation)
    {
        Data.Orientation = newOrientation;
        Renderer.UpdateLemmingState(true);
    }

    public void SetCurrentAction(LemmingAction lemmingAction)
    {
        CurrentAction = lemmingAction;
        Renderer.UpdateLemmingState(true);
    }

    public void SetNextAction(LemmingAction nextAction)
    {
        NextAction = nextAction;
    }

    public void SetCountDownAction(int countDownTimer, LemmingAction countDownAction, bool displayTimer)
    {
        Data.CountDownTimer = countDownTimer;
        CountDownAction = countDownAction;

        Renderer.SetDisplayTimer(displayTimer);
    }

    public void ClearCountDownAction()
    {
        Data.CountDownTimer = 0;
        CountDownAction = NoneAction.Instance;
    }

    public void OnUpdatePosition()
    {
        Renderer.UpdatePosition();
    }

    public void OnRemoval(LemmingRemovalReason removalReason)
    {
        CurrentAction = NoneAction.Instance;
        Renderer.UpdateLemmingState(removalReason == LemmingRemovalReason.DeathExplode);
    }

    public Span<Point> GetJumperPositions() => Data.JumperPositionBuffer;

    public unsafe void SetRawDataFromOther(Lemming otherLemming)
    {
        otherLemming.UpdateRawData();

        fixed (void* otherPointer = &otherLemming.Data)
        fixed (void* thisPointer = &Data)
        {
            var sourceSpan = new ReadOnlySpan<byte>(otherPointer, sizeof(LemmingData));
            var destinationSpan = new Span<byte>(thisPointer, sizeof(LemmingData));

            sourceSpan.CopyTo(destinationSpan);
        }

        SetFromRawData();

        Renderer.ResetPosition();
        LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
    }

    public void SetRawData(Tribe tribe, uint rawStateData, Orientation orientation, FacingDirection facingDirection)
    {
        State.SetData(tribe.Id, rawStateData);
        Data.Orientation = orientation;
        Data.FacingDirection = facingDirection;
    }

    public unsafe int GetRequiredNumberOfBytesForSnapshotting() => sizeof(LemmingData);

    public unsafe void WriteToSnapshotData(byte* snapshotDataPointer)
    {
        UpdateRawData();

        fixed (void* thisPointer = &Data)
        {
            var sourceSpan = new ReadOnlySpan<byte>(thisPointer, sizeof(LemmingData));
            var destinationSpan = new Span<byte>(snapshotDataPointer, sizeof(LemmingData));

            sourceSpan.CopyTo(destinationSpan);
        }
    }

    public unsafe void SetFromSnapshotData(byte* snapshotDataPointer)
    {
        fixed (void* thisPointer = &Data)
        {
            var sourceSpan = new ReadOnlySpan<byte>(snapshotDataPointer, sizeof(LemmingData));
            var destinationSpan = new Span<byte>(thisPointer, sizeof(LemmingData));

            sourceSpan.CopyTo(destinationSpan);
        }

        SetFromRawData();

        Renderer.ResetPosition();
        LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
    }

    private void UpdateRawData()
    {
        State.WriteToSnapshotData(out Data.TribeId, out Data.State);

        Data.PreviousActionId = PreviousAction.Id;
        Data.CurrentActionId = CurrentAction.Id;
        Data.NextActionId = NextAction.Id;
        Data.CountDownActionId = CountDownAction.Id;
    }

    private void SetFromRawData()
    {
        State.SetFromSnapshotData(Data.TribeId, Data.State);

        PreviousAction = LemmingAction.GetActionOrDefault(Data.PreviousActionId);
        CurrentAction = LemmingAction.GetActionOrDefault(Data.CurrentActionId);
        NextAction = LemmingAction.GetActionOrDefault(Data.NextActionId);
        CountDownAction = LemmingAction.GetActionOrDefault(Data.CountDownActionId);
    }

    int IIdEquatable<Lemming>.Id => Id;

    [DebuggerStepThrough]
    public bool Equals(Lemming? other) => Id == (other?.Id ?? -1);
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Lemming other && Id == other.Id;
    [DebuggerStepThrough]
    public override int GetHashCode() => Id;

    [DebuggerStepThrough]
    public static bool operator ==(Lemming left, Lemming right) => left.Id == right.Id;
    [DebuggerStepThrough]
    public static bool operator !=(Lemming left, Lemming right) => left.Id != right.Id;
}
