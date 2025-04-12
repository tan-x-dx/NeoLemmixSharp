using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class Lemming : IIdEquatable<Lemming>, IRectangularBounds, ISnapshotDataConvertible<LemmingSnapshotData>
{
    public static Lemming SimulationLemming { get; } = new();

    public readonly int Id;
    private JumperPositionBuffer _jumperPositionBuffer;

    public bool ConstructivePositionFreeze;
    public bool IsStartingAction;
    public bool PlacedBrick;
    public bool StackLow;

    public bool InitialFall;
    public bool EndOfAnimation;
    public bool LaserHit;
    public bool JumpToHoistAdvance;

    public int AnimationFrame;
    public int PhysicsFrame;
    public int AscenderProgress;
    public int NumberOfBricksLeft;
    public int DisarmingFrames;
    public int DistanceFallen;
    public int JumpProgress;
    public int TrueDistanceFallen;
    public int LaserRemainTime;

    public int FastForwardTime;
    public int CountDownTimer;
    public int ParticleTimer;

    public Point DehoistPin = new(-1, -1);
    public Point LaserHitLevelPosition = new(-1, -1);
    public Point AnchorPosition = new(-1, -1);
    public Point PreviousLevelPosition = new(-1, -1);

    public RectangularRegion CurrentBounds { get; private set; }

    public LemmingState State { get; }

    public FacingDirection FacingDirection { get; private set; } = FacingDirection.Right;
    public Orientation Orientation { get; private set; } = Orientation.Down;

    public LemmingAction PreviousAction { get; private set; } = NoneAction.Instance;
    public LemmingAction CurrentAction { get; private set; }
    public LemmingAction NextAction { get; private set; } = NoneAction.Instance;
    public LemmingAction CountDownAction { get; private set; } = NoneAction.Instance;

    public LemmingRenderer Renderer { get; }

    public bool IsSimulation => Id < 0;
    public bool IsFastForward => FastForwardTime > 0 || State.IsPermanentFastForwards;

    public Point HeadPosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Orientation.MoveUp(AnchorPosition, 6);
    }

    public Point FootPosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => CurrentAction.GetFootPosition(this, AnchorPosition);
    }

    public Point CenterPosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Orientation.MoveUp(AnchorPosition, 4);
    }

    public Lemming(
        int id,
        Orientation orientation,
        FacingDirection facingDirection,
        LemmingAction currentAction,
        int teamId)
    {
        Id = id;
        Orientation = orientation;
        FacingDirection = facingDirection;
        CurrentAction = currentAction;
        State = new LemmingState(this, teamId);
        Renderer = new LemmingRenderer(this);
    }

    private Lemming()
    {
        Id = -1;
        Orientation = Orientation.Down;
        FacingDirection = FacingDirection.Right;
        CurrentAction = NoneAction.Instance;
        State = new LemmingState(this, EngineConstants.ClassicTeamId);
        Renderer = new LemmingRenderer(this);
    }

    public void Initialise()
    {
        State.IsActive = true;
        PreviousLevelPosition = AnchorPosition;
        CurrentBounds = CurrentAction.GetLemmingBounds(this);

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
        var p = Orientation.Move(AnchorPosition, -5, -12);
        gadgetCheckPositions[0] = p;
        p = Orientation.Move(AnchorPosition, 5, 12);
        gadgetCheckPositions[1] = p;
        p = PreviousLevelPosition;
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
        gadgetCheckPositions[0] = Orientation.Move(AnchorPosition, -8, -16);
        gadgetCheckPositions[1] = Orientation.Move(AnchorPosition, 8, 16);
        gadgetCheckPositions[2] = PreviousLevelPosition;
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
        if (ParticleTimer > 0)
        {
            ParticleTimer--;
        }
    }

    private void HandleCountDownTimer()
    {
        if (CountDownTimer == 0)
            return;

        CountDownTimer--;
        CountDownHelper.UpdateCountDownTimer(this);

        if (CountDownTimer != 0)
            return;

        OhNoerAction.HandleCountDownTransition(this);
    }

    private void HandleFastForwardTimer()
    {
        if (FastForwardTime > 0)
        {
            FastForwardTime--;
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
        var frame = AnimationFrame + 1;
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
        AnimationFrame = frame;

        frame = PhysicsFrame + 1;
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

            EndOfAnimation = CurrentAction.IsOneTimeAction();
        }
        PhysicsFrame = frame;

        PreviousLevelPosition = AnchorPosition;

        var result = CurrentAction.UpdateLemming(this, in gadgetsNearLemming);
        CurrentBounds = CurrentAction.GetLemmingBounds(this);

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
            PreviousLevelPosition = AnchorPosition;
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
                if (!gadget.ContainsEitherPoint(Orientation, anchorPosition, footPosition))
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

                AnchorPosition = anchorPosition;

                CurrentBounds = afterAction.GetLemmingBounds(this);

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
            checkPosition == AnchorPosition &&
            (NextAction != SplatterAction.Instance ||
            filter.HitBoxBehaviour != HitBoxBehaviour.Liquid))
        {
            NextAction.TransitionLemmingToAction(this, false);
            if (JumpToHoistAdvance)
            {
                AnimationFrame += 2;
                PhysicsFrame += 2;
                JumpToHoistAdvance = false;
            }

            NextAction = NoneAction.Instance;
        }

        gadget.OnLemmingHit(filter, this);
    }

    public void SetFacingDirection(FacingDirection newFacingDirection)
    {
        FacingDirection = newFacingDirection;
        Renderer.UpdateLemmingState(true);
    }

    public void SetOrientation(Orientation newOrientation)
    {
        Orientation = newOrientation;
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
        CountDownTimer = countDownTimer;
        CountDownAction = countDownAction;

        Renderer.SetDisplayTimer(displayTimer);
    }

    public void ClearCountDownAction()
    {
        CountDownTimer = 0;
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

    public Span<Point> GetJumperPositions() => _jumperPositionBuffer;

    public void SetRawDataFromOther(Lemming otherLemming)
    {
        _jumperPositionBuffer = otherLemming._jumperPositionBuffer;

        ConstructivePositionFreeze = otherLemming.ConstructivePositionFreeze;
        IsStartingAction = otherLemming.IsStartingAction;
        PlacedBrick = otherLemming.PlacedBrick;
        StackLow = otherLemming.StackLow;
        InitialFall = otherLemming.InitialFall;
        EndOfAnimation = otherLemming.EndOfAnimation;
        LaserHit = otherLemming.LaserHit;
        JumpToHoistAdvance = otherLemming.JumpToHoistAdvance;

        AnimationFrame = otherLemming.AnimationFrame;
        PhysicsFrame = otherLemming.PhysicsFrame;
        AscenderProgress = otherLemming.AscenderProgress;
        NumberOfBricksLeft = otherLemming.NumberOfBricksLeft;
        DisarmingFrames = otherLemming.DisarmingFrames;
        DistanceFallen = otherLemming.DistanceFallen;
        JumpProgress = otherLemming.JumpProgress;
        TrueDistanceFallen = otherLemming.TrueDistanceFallen;
        LaserRemainTime = otherLemming.LaserRemainTime;

        FastForwardTime = otherLemming.FastForwardTime;
        CountDownTimer = otherLemming.CountDownTimer;

        DehoistPin = otherLemming.DehoistPin;
        LaserHitLevelPosition = otherLemming.LaserHitLevelPosition;
        AnchorPosition = otherLemming.AnchorPosition;
        PreviousLevelPosition = otherLemming.PreviousLevelPosition;

        FacingDirection = otherLemming.FacingDirection;
        Orientation = otherLemming.Orientation;

        PreviousAction = otherLemming.PreviousAction;
        CurrentAction = otherLemming.CurrentAction;
        NextAction = otherLemming.NextAction;

        State.SetRawDataFromOther(otherLemming.State);

        CurrentBounds = otherLemming.CurrentBounds;

        Renderer.ResetPosition();
        LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
    }

    public void SetRawDataFromOther(Team team, uint rawStateData, Orientation orientation, FacingDirection facingDirection)
    {
        State.SetRawDataFromOther(rawStateData);
        State.TeamAffiliation = team;
        Orientation = orientation;
        FacingDirection = facingDirection;
    }

    public void WriteToSnapshotData(out LemmingSnapshotData lemmingSnapshotData)
    {
        lemmingSnapshotData = new LemmingSnapshotData(this);
    }

    public void SetFromSnapshotData(in LemmingSnapshotData lemmingSnapshotData)
    {
        if (Id != lemmingSnapshotData.Id)
            throw new InvalidOperationException("Mismatching IDs!");

        _jumperPositionBuffer = lemmingSnapshotData.JumperPositionBuffer;

        ConstructivePositionFreeze = lemmingSnapshotData.ConstructivePositionFreeze;
        IsStartingAction = lemmingSnapshotData.IsStartingAction;
        PlacedBrick = lemmingSnapshotData.PlacedBrick;
        StackLow = lemmingSnapshotData.StackLow;

        InitialFall = lemmingSnapshotData.InitialFall;
        EndOfAnimation = lemmingSnapshotData.EndOfAnimation;
        LaserHit = lemmingSnapshotData.LaserHit;
        JumpToHoistAdvance = lemmingSnapshotData.JumpToHoistAdvance;

        AnimationFrame = lemmingSnapshotData.AnimationFrame;
        PhysicsFrame = lemmingSnapshotData.PhysicsFrame;
        AscenderProgress = lemmingSnapshotData.AscenderProgress;
        NumberOfBricksLeft = lemmingSnapshotData.NumberOfBricksLeft;
        DisarmingFrames = lemmingSnapshotData.DisarmingFrames;
        DistanceFallen = lemmingSnapshotData.DistanceFallen;
        JumpProgress = lemmingSnapshotData.JumpProgress;
        TrueDistanceFallen = lemmingSnapshotData.TrueDistanceFallen;
        LaserRemainTime = lemmingSnapshotData.LaserRemainTime;

        FastForwardTime = lemmingSnapshotData.FastForwardTime;
        CountDownTimer = lemmingSnapshotData.CountDownTimer;
        ParticleTimer = lemmingSnapshotData.ParticleTimer;

        DehoistPin = lemmingSnapshotData.DehoistPin;
        LaserHitLevelPosition = lemmingSnapshotData.LaserHitLevelPosition;
        AnchorPosition = lemmingSnapshotData.LevelPosition;
        PreviousLevelPosition = lemmingSnapshotData.PreviousLevelPosition;

        CurrentBounds = lemmingSnapshotData.CurrentBounds;

        State.SetFromSnapshotData(in lemmingSnapshotData.StateSnapshotData);

        FacingDirection = lemmingSnapshotData.FacingDirection;
        Orientation = lemmingSnapshotData.Orientation;

        PreviousAction = LemmingAction.GetActionFromUnboundId(lemmingSnapshotData.PreviousActionId);
        CurrentAction = LemmingAction.GetActionFromUnboundId(lemmingSnapshotData.CurrentActionId);
        NextAction = LemmingAction.GetActionFromUnboundId(lemmingSnapshotData.NextActionId);
        CountDownAction = LemmingAction.GetActionFromUnboundId(lemmingSnapshotData.CountDownActionId);

        Renderer.ResetPosition();
        LevelScreen.LemmingManager.UpdateLemmingFastForwardState(this);
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

    [InlineArray(JumperAction.JumperPositionCount)]
    public struct JumperPositionBuffer
    {
        private Point _0;
    }
}