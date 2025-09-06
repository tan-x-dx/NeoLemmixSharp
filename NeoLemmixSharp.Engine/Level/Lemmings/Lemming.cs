﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class Lemming : IIdEquatable<Lemming>, IRectangularBounds, ISnapshotDataConvertible
{
    public static Lemming SimulationLemming { get; } = new();

    public readonly int Id;

    private LemmingData _data;

    private LemmingAction _previousAction = NoneAction.Instance;
    private LemmingAction _currentAction;
    private LemmingAction _nextAction = NoneAction.Instance;
    private LemmingAction _countDownAction = NoneAction.Instance;

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
    public ref int FastForwardTime => ref _data.FastForwardTime;
    public ref int CountDownTimer => ref _data.CountDownTimer;
    public ref int ParticleTimer => ref _data.ParticleTimer;

    public LemmingState State { get; }

    public LemmingAction PreviousAction
    {
        get => _previousAction;
        private set => _previousAction = value;
    }
    public LemmingAction CurrentAction
    {
        get => _currentAction;
        set
        {
            _currentAction = value;
            Renderer.UpdateLemmingState(true);
        }
    }
    public LemmingAction NextAction
    {
        get => _nextAction;
        set => _nextAction = value;
    }
    public LemmingAction CountDownAction
    {
        get => _countDownAction;
        set => _countDownAction = value;
    }

    public LemmingRenderer Renderer { get; }

    public bool IsSimulation => Id < 0;
    public bool IsFastForward => _data.FastForwardTime > 0 || State.IsPermanentFastForwards;

    public Point HeadPosition => _data.Orientation.MoveUp(_data.AnchorPosition, 6);
    public Point FootPosition => CurrentAction.GetFootPosition(this, _data.AnchorPosition);
    public Point CenterPosition => _data.Orientation.MoveUp(_data.AnchorPosition, 4);

    public Lemming(
        int id,
        Orientation orientation,
        FacingDirection facingDirection,
        int initialActionId,
        int tribeId)
    {
        Id = id;
        _data.Orientation = orientation;
        _data.FacingDirection = facingDirection;
        _currentAction = LemmingAction.GetActionOrDefault(initialActionId);
        _data.CurrentActionId = _currentAction.Id;
        State = new LemmingState(this, tribeId);
        Renderer = new LemmingRenderer(this);

        UpdateRawData();
    }

    private Lemming()
    {
        Id = -1;
        _data.Orientation = Orientation.Down;
        _data.FacingDirection = FacingDirection.Right;
        _currentAction = NoneAction.Instance;
        State = new LemmingState(this, EngineConstants.ClassicTribeId);
        Renderer = new LemmingRenderer(this);

        UpdateRawData();
    }

    public void Initialise()
    {
        State.IsActive = true;
        _data.PreviousAnchorPosition = _data.AnchorPosition;
        _data.CurrentBounds = CurrentAction.GetLemmingBounds(this);

        var initialAction = CurrentAction;
        if (initialAction == NoneAction.Instance)
        {
            initialAction = WalkerAction.Instance;
        }

        initialAction.TransitionLemmingToAction(this, false);

        UpdateRawData();
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
        var p = _data.Orientation.Move(_data.AnchorPosition, -5, -12);
        gadgetCheckPositions[0] = p;
        p = _data.Orientation.Move(_data.AnchorPosition, 5, 12);
        gadgetCheckPositions[1] = p;
        p = _data.PreviousAnchorPosition;
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
        gadgetCheckPositions[0] = _data.Orientation.Move(_data.AnchorPosition, -8, -16);
        gadgetCheckPositions[1] = _data.Orientation.Move(_data.AnchorPosition, 8, 16);
        gadgetCheckPositions[2] = _data.PreviousAnchorPosition;
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
        if (_data.FastForwardTime > 0)
        {
            _data.FastForwardTime--;
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
        var frame = _data.AnimationFrame + 1;
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
        _data.AnimationFrame = frame;

        frame = _data.PhysicsFrame + 1;
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

            _data.EndOfAnimation = CurrentAction.IsOneTimeAction();
        }

        _data.PhysicsFrame = frame;
        _data.PreviousAnchorPosition = _data.AnchorPosition;

        var result = CurrentAction.UpdateLemming(this, in gadgetsNearLemming);
        _data.CurrentBounds = CurrentAction.GetLemmingBounds(this);

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
        if (NextAction != NoneAction.Instance &&
            checkPosition == _data.AnchorPosition &&
            (NextAction != SplatterAction.Instance ||
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

    public void SetCountDownAction(int countDownTimer, LemmingAction countDownAction, bool displayTimer)
    {
        _data.CountDownTimer = countDownTimer;
        CountDownAction = countDownAction;

        Renderer.SetDisplayTimer(displayTimer);
    }

    public void ClearCountDownAction()
    {
        _data.CountDownTimer = 0;
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

    public Span<Point> GetJumperPositions() => _data.JumperPositionBuffer;

    public unsafe void SetRawDataFromOther(Lemming otherLemming)
    {
        otherLemming.UpdateRawData();

        fixed (void* otherPointer = &otherLemming._data)
        fixed (void* thisPointer = &_data)
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
        _data.Orientation = orientation;
        _data.FacingDirection = facingDirection;
    }

    public unsafe int GetRequiredNumberOfBytesForSnapshotting() => sizeof(LemmingData);

    public unsafe void WriteToSnapshotData(byte* snapshotDataPointer)
    {
        UpdateRawData();

        fixed (void* thisPointer = &_data)
        {
            var sourceSpan = new ReadOnlySpan<byte>(thisPointer, sizeof(LemmingData));
            var destinationSpan = new Span<byte>(snapshotDataPointer, sizeof(LemmingData));

            sourceSpan.CopyTo(destinationSpan);
        }
    }

    public unsafe void SetFromSnapshotData(byte* snapshotDataPointer)
    {
        fixed (void* thisPointer = &_data)
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
        State.WriteToSnapshotData(out _data.TribeId, out _data.State);

        _data.PreviousActionId = PreviousAction.Id;
        _data.CurrentActionId = CurrentAction.Id;
        _data.NextActionId = NextAction.Id;
        _data.CountDownActionId = CountDownAction.Id;
    }

    private void SetFromRawData()
    {
        State.SetFromSnapshotData(_data.TribeId, _data.State);

        PreviousAction = LemmingAction.GetActionOrDefault(_data.PreviousActionId);
        CurrentAction = LemmingAction.GetActionOrDefault(_data.CurrentActionId);
        NextAction = LemmingAction.GetActionOrDefault(_data.NextActionId);
        CountDownAction = LemmingAction.GetActionOrDefault(_data.CountDownActionId);
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
