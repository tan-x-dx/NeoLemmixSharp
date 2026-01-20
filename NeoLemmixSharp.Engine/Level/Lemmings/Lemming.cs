using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class Lemming : IEquatable<Lemming>, IRectangularBounds
{
    public LemmingState State { get; }
    public LemmingRenderer Renderer { get; }

    private readonly LemmingData _data;

    public readonly int Id;

    public int PreviousActionId => _data.PreviousActionId;
    public LemmingAction PreviousAction => LemmingAction.GetActionOrDefault(_data.PreviousActionId);

    public int CurrentActionId => _data.CurrentActionId;
    public LemmingAction CurrentAction
    {
        get => LemmingAction.GetActionOrDefault(_data.CurrentActionId);
        set
        {
            _data.CurrentActionId = value.Id;
            Renderer.UpdateLemmingState(true);
        }
    }

    public int NextActionId => _data.NextActionId;
    public LemmingAction NextAction
    {
        get => LemmingAction.GetActionOrDefault(_data.NextActionId);
        set => _data.NextActionId = value.Id;
    }

    public LemmingAction CountDownAction => LemmingAction.GetActionOrDefault(_data.CountDownActionId);

    public Orientation Orientation
    {
        get => _data.Orientation;
        set
        {
            _data.Orientation = value;
            Renderer.UpdateLemmingState(true);
        }
    }

    public FacingDirection FacingDirection
    {
        get => _data.FacingDirection;
        set
        {
            _data.FacingDirection = value;
            Renderer.UpdateLemmingState(true);
        }
    }

    public RectangularRegion CurrentBounds
    {
        get => _data.CurrentBounds;
        set => _data.CurrentBounds = value;
    }

    public ref Point DehoistPin => ref _data.DehoistPin;
    public ref Point LaserHitLevelPosition => ref _data.LaserHitLevelPosition;
    public ref Point AnchorPosition => ref _data.AnchorPosition;
    public ref Point PreviousAnchorPosition => ref _data.PreviousAnchorPosition;

    public ref bool ConstructivePositionFreeze => ref _data.ConstructivePositionFreeze;
    public ref bool IsStartingAction => ref _data.IsStartingAction;
    public ref bool PlacedBrick => ref _data.PlacedBrick;
    public ref bool StackLow => ref _data.StackLow;
    public ref bool InitialFall => ref _data.InitialFall;
    public ref bool EndOfAnimation => ref _data.EndOfAnimation;
    public ref bool LaserHit => ref _data.LaserHit;
    public ref bool JumpToHoistAdvance => ref _data.JumpToHoistAdvance;
    public ref int AnimationFrame => ref _data.AnimationFrame;
    public ref int PhysicsFrame => ref _data.PhysicsFrame;
    public ref int AscenderProgress => ref _data.AscenderProgress;
    public ref int NumberOfBricksLeft => ref _data.NumberOfBricksLeft;
    public ref int DisarmingFrames => ref _data.DisarmingFrames;
    public ref int DistanceFallen => ref _data.DistanceFallen;
    public ref int JumpProgress => ref _data.JumpProgress;
    public ref int TrueDistanceFallen => ref _data.TrueDistanceFallen;
    public ref int LaserRemainTime => ref _data.LaserRemainTime;
    public ref uint CountDownTimer => ref _data.CountDownTimer;
    public ref int ParticleTimer => ref _data.ParticleTimer;

    public bool IsSimulation => Id == EngineConstants.SimulationLemmingId;
    public bool IsFastForward => _data.FastForwardTime > 0 || State.IsPermanentFastForwards;

    public Point HeadPosition => _data.Orientation.MoveUp(_data.AnchorPosition, 6);
    public Point FootPosition => CurrentAction.GetFootPosition(this, _data.AnchorPosition);
    public Point CenterPosition => _data.Orientation.MoveUp(_data.AnchorPosition, 4);

    public Span<Point> GetJumperPositions() => _data.GetJumperPositions();

    public Lemming(
        ref nint dataHandle,
        int id)
    {
        Id = id;
        _data = PointerDataHelper.CreateItem<LemmingData>(ref dataHandle);
        _data.PreviousActionId = LemmingActionConstants.NoneActionId;
        _data.CurrentActionId = LemmingActionConstants.NoneActionId;
        _data.NextActionId = LemmingActionConstants.NoneActionId;
        _data.CountDownActionId = LemmingActionConstants.NoneActionId;
        _data.DehoistPin = new(-1, -1);
        _data.LaserHitLevelPosition = new(-1, -1);
        _data.AnchorPosition = new(-1, -1);
        _data.PreviousAnchorPosition = new(-1, -1);

        State = _data.CreateLemmingState(this);
        Renderer = new LemmingRenderer(this);
    }

    public void Initialise()
    {
        State.IsActive = true;
        _data.PreviousAnchorPosition = _data.AnchorPosition;

        var initialAction = CurrentAction;
        _data.CurrentBounds = initialAction.GetLemmingBounds(this);

        if (initialAction.Id == LemmingActionConstants.NoneActionId)
        {
            initialAction = WalkerAction.Instance;
        }

        initialAction.TransitionLemmingToAction(this, false);

        Renderer.UpdateLemmingState(true);
    }

    [SkipLocalsInit]
    public void Tick()
    {
        _data.PreviousActionId = _data.CurrentActionId;
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
        var p = _data.Orientation.Move(_data.AnchorPosition, -5, -12);
        gadgetCheckPositions[0] = p;
        p = _data.Orientation.Move(_data.AnchorPosition, 5, 12);
        gadgetCheckPositions[1] = p;
        p = _data.PreviousAnchorPosition;
        gadgetCheckPositions[2] = p;
        p = PreviousAction.GetFootPosition(this, p);
        gadgetCheckPositions[3] = p;

        var checkPositionsBounds = new RectangularRegion(gadgetCheckPositions[..4]);

        LevelScreen.GadgetManager.GetAllItemsNearRegion(checkPositionsBounds, out var gadgetsNearLemming);

        EvaluateLemmingLogic(gadgetCheckPositions, in gadgetsNearLemming);
    }

    private void EvaluateLemmingLogic(
        Span<Point> gadgetCheckPositions,
        in GadgetEnumerable gadgetsNearLemming)
    {
        if (!HandleLemmingAction(in gadgetsNearLemming)) return;
        if (!CheckLevelBoundaries()) return;
        if (!CheckTriggerAreas(false, gadgetCheckPositions, in gadgetsNearLemming)) return;
        if (CurrentActionId == LemmingActionConstants.ExiterActionId) return;
        if (State.IsZombie) return;
        if (!LevelScreen.LemmingManager.AnyZombies()) return;

        LevelScreen.LemmingManager.DoZombieCheck(this);
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
        gadgetCheckPositions[0] = _data.Orientation.Move(_data.AnchorPosition, -8, -16);
        gadgetCheckPositions[1] = _data.Orientation.Move(_data.AnchorPosition, 8, 16);
        gadgetCheckPositions[2] = _data.PreviousAnchorPosition;
        gadgetCheckPositions[3] = PreviousAction.GetFootPosition(this, gadgetCheckPositions[2]);

        var checkPositionsBounds = new RectangularRegion(gadgetCheckPositions[..4]);

        LevelScreen.GadgetManager.GetAllItemsNearRegion(checkPositionsBounds, out var gadgetsNearLemming);

        var handleGadgets = HandleLemmingAction(in gadgetsNearLemming) && CheckLevelBoundaries() && checkGadgets;
        if (handleGadgets)
        {
            // Reuse the above span. LemmingMovementHelper will overwrite existing values
            CheckTriggerAreas(false, gadgetCheckPositions, in gadgetsNearLemming);
        }
    }

    private void HandleParticleTimer()
    {
        if (_data.ParticleTimer > 0)
        {
            _data.ParticleTimer--;
        }
    }

    private void HandleCountDownTimer()
    {
        if (_data.CountDownTimer == 0)
            return;

        _data.CountDownTimer--;
        CountDownHelper.UpdateCountDownTimer(this);

        if (_data.CountDownTimer != 0)
            return;

        OhNoerAction.HandleCountDownTransition(this);
    }

    private void HandleFastForwardTimer()
    {
        ref var fastForwardTime = ref _data.FastForwardTime;

        if (fastForwardTime <= 0)
            return;

        fastForwardTime--;

        if (fastForwardTime == 0)
            LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
    }

    /// <summary>
    /// Handle one frame of the lemming's action, and update data accordingly.
    /// Some actions (e.g. exiter, splatter, etc) may result in the lemming being killed off in some way.
    /// In that case, the method returns false - no extra work is necessary (don't bother checking gadgets, for example).
    /// </summary>
    /// <returns>True if more work needs to be done this frame</returns>
    private bool HandleLemmingAction(in GadgetEnumerable gadgetsNearLemming)
    {
        var currentAction = CurrentAction;

        var frame = _data.AnimationFrame + 1;
        if (frame == currentAction.NumberOfAnimationFrames)
        {
            // Floater and Glider start cycle at frame 9!
            if (currentAction.Id == LemmingActionConstants.FloaterActionId ||
                currentAction.Id == LemmingActionConstants.GliderActionId)
            {
                frame = EngineConstants.FloaterGliderStartCycleFrame;
            }
            else
            {
                frame = 0;
            }
        }
        _data.AnimationFrame = frame;

        frame = _data.PhysicsFrame + 1;
        if (frame == currentAction.MaxPhysicsFrames)
        {
            // Floater and Glider start cycle at frame 9!
            if (currentAction.Id == LemmingActionConstants.FloaterActionId ||
                currentAction.Id == LemmingActionConstants.GliderActionId)
            {
                frame = EngineConstants.FloaterGliderStartCycleFrame;
            }
            else
            {
                frame = 0;
            }

            _data.EndOfAnimation = currentAction.IsOneTimeAction();
        }

        _data.PhysicsFrame = frame;
        _data.PreviousAnchorPosition = _data.AnchorPosition;

        var result = currentAction.UpdateLemming(this, in gadgetsNearLemming);
        _data.CurrentBounds = currentAction.GetLemmingBounds(this);

        return result;
    }

    private bool CheckLevelBoundaries()
    {
        var terrainManager = LevelScreen.TerrainManager;
        var footPixel = terrainManager.PixelTypeAtPosition(FootPosition);
        var headPixel = terrainManager.PixelTypeAtPosition(HeadPosition);

        if ((footPixel & headPixel).IsVoid())
        {
            LevelScreen.LemmingManager.RemoveLemming(this, LemmingRemovalReason.DeathVoid);
            return false;
        }

        return true;
    }

    private bool CheckTriggerAreas(
        bool isPostTeleportCheck,
        Span<Point> gadgetCheckPositions,
        in GadgetEnumerable gadgetsNearLemming)
    {
        if (isPostTeleportCheck)
        {
            _data.PreviousAnchorPosition = _data.AnchorPosition;
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
                if (!gadget.ContainsEitherPoint(_data.Orientation, anchorPosition, footPosition))
                    continue;

                var firstMatchingFilter = GetFirstMatchingLemmingFilter(currentState.Filters);
                if (firstMatchingFilter is null)
                    continue;

                var beforeAction = CurrentAction;
                HandleGadgetInteraction(gadget, firstMatchingFilter, anchorPosition);
                var afterAction = CurrentAction;

                if (beforeAction != afterAction)
                {
                    _data.AnchorPosition = anchorPosition;
                    _data.CurrentBounds = afterAction.GetLemmingBounds(this);

                    return false;
                }
            }
        }

        return true;
    }

    private LemmingHitBoxFilter? GetFirstMatchingLemmingFilter(ReadOnlySpan<LemmingHitBoxFilter> filters)
    {
        foreach (var filter in filters)
        {
            if (filter.MatchesLemming(this))
            {
                return filter;
            }
        }

        return null;
    }

    private void HandleGadgetInteraction(
        HitBoxGadget gadget,
        LemmingHitBoxFilter filter,
        Point checkPosition)
    {
        // If we're at the end of the check positions and Next action is not None
        // then transition. However, if NextAction is SplatterAction and there's water
        // at the position, the water takes precedence over splatting
        if (NextActionId != LemmingActionConstants.NoneActionId &&
            checkPosition == _data.AnchorPosition &&
            (NextActionId != LemmingActionConstants.SplatterActionId ||
            filter.HitBoxBehaviour != HitBoxInteractionType.Liquid))
        {
            NextAction.TransitionLemmingToAction(this, false);
            if (_data.JumpToHoistAdvance)
            {
                _data.AnimationFrame += 2;
                _data.PhysicsFrame += 2;
                _data.JumpToHoistAdvance = false;
            }

            NextAction = NoneAction.Instance;
        }

        gadget.OnLemmingHit(filter, this);
    }

    public void SetFastForwardTime(int fastForwardTime)
    {
        _data.FastForwardTime = fastForwardTime;
        LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
    }

    public void SetCountDownAction(uint countDownTimer, LemmingAction countDownAction, bool displayTimer)
    {
        _data.CountDownTimer = countDownTimer;
        _data.CountDownActionId = countDownAction.Id;

        Renderer.SetDisplayTimer(displayTimer);
    }

    public void ClearCountDownAction()
    {
        _data.CountDownTimer = 0;
        _data.CountDownActionId = LemmingActionConstants.NoneActionId;
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

    public unsafe void SetRawDataFromOther(Lemming otherLemming)
    {
        void* otherPointer = otherLemming._data.GetPointer();
        void* thisPointer = _data.GetPointer();
        CopyLemmingSnapshotBytes(otherPointer, thisPointer);
        OnSnapshotApplied();
    }

    private static unsafe void CopyLemmingSnapshotBytes(void* sourcePointer, void* destinationPointer)
    {
        var sourceSpan = Helpers.CreateReadOnlySpan<byte>(sourcePointer, LemmingData.SizeInBytes);
        var destinationSpan = Helpers.CreateSpan<byte>(destinationPointer, LemmingData.SizeInBytes);
        sourceSpan.CopyTo(destinationSpan);
    }

    public void SetRawData(Orientation orientation, FacingDirection facingDirection, int tribeId, uint rawStateData)
    {
        _data.Orientation = orientation;
        _data.FacingDirection = facingDirection;
        _data.TribeId = tribeId;
        _data.State = rawStateData;

        OnSnapshotApplied();
    }

    public void OnSnapshotApplied()
    {
        State.UpdateHairAndBodyColors();
        State.UpdateSkinColor();
        State.UpdatePaintColor();
        Renderer.UpdateLemmingState(State.IsActive);
        LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
        LevelScreen.LemmingManager.UpdateZombieState(this);
    }

    [DebuggerStepThrough]
    public bool Equals(Lemming? other)
    {
        var otherValue = -1;
        if (other is not null) otherValue = other.Id;
        return Id == otherValue;
    }
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Lemming other && Id == other.Id;
    [DebuggerStepThrough]
    public override int GetHashCode() => Id;

    [DebuggerStepThrough]
    public static bool operator ==(Lemming? left, Lemming? right)
    {
        var leftValue = -1;
        if (left is not null) leftValue = left.Id;
        var rightValue = -1;
        if (right is not null) rightValue = right.Id;
        return leftValue == rightValue;
    }
    [DebuggerStepThrough]
    public static bool operator !=(Lemming? left, Lemming? right) => !(left == right);
}
