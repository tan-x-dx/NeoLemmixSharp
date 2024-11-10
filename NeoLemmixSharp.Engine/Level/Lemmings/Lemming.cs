using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
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

    public LevelPosition DehoistPin = new(-1, -1);
    public LevelPosition LaserHitLevelPosition = new(-1, -1);
    public LevelPosition LevelPosition = new(-1, -1);
    public LevelPosition PreviousLevelPosition = new(-1, -1);
    public LevelPosition TopLeftPixel { get; private set; }
    public LevelPosition BottomRightPixel { get; private set; }
    public LevelPosition PreviousTopLeftPixel { get; private set; }
    public LevelPosition PreviousBottomRightPixel { get; private set; }

    public LemmingState State { get; }

    public FacingDirection FacingDirection { get; private set; }
    public Orientation Orientation { get; private set; }

    public LemmingAction PreviousAction { get; private set; } = NoneAction.Instance;
    public LemmingAction CurrentAction { get; private set; }
    public LemmingAction NextAction { get; private set; } = NoneAction.Instance;
    public LemmingAction CountDownAction { get; private set; } = NoneAction.Instance;

    public LemmingRenderer Renderer { get; }

    public bool IsSimulation => Id < 0;
    public bool IsFastForward => FastForwardTime > 0 || State.IsPermanentFastForwards;

    public LevelPosition HeadPosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Orientation.MoveUp(LevelPosition, 6);
    }

    public LevelPosition FootPosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => CurrentAction.GetFootPosition(this, LevelPosition);
    }

    public LevelPosition CenterPosition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Orientation.MoveUp(LevelPosition, 4);
    }

    public Lemming(
        int id,
        Orientation orientation,
        FacingDirection facingDirection,
        LemmingAction currentAction,
        Team team)
    {
        Id = id;
        Orientation = orientation;
        FacingDirection = facingDirection;
        CurrentAction = currentAction;
        State = new LemmingState(this, team);
        Renderer = new LemmingRenderer(this);
    }

    private Lemming()
    {
        Id = -1;
        Orientation = DownOrientation.Instance;
        FacingDirection = FacingDirection.RightInstance;
        CurrentAction = NoneAction.Instance;
        State = new LemmingState(this, Team.AllItems[EngineConstants.ClassicTeamId]);
        Renderer = new LemmingRenderer(this);
    }

    public void Initialise()
    {
        State.IsActive = true;
        var lemmingBounds = CurrentAction.GetLemmingBounds(this);
        TopLeftPixel = lemmingBounds.GetTopLeftPosition();
        BottomRightPixel = lemmingBounds.GetBottomRightPosition();

        PreviousLevelPosition = LevelPosition;
        PreviousTopLeftPixel = TopLeftPixel;
        PreviousBottomRightPixel = BottomRightPixel;

        var initialAction = CurrentAction;
        if (initialAction == NoneAction.Instance)
        {
            initialAction = WalkerAction.Instance;
        }

        initialAction.TransitionLemmingToAction(this, false);
        Renderer.UpdateLemmingState(true);
    }

    public void Tick()
    {
        PreviousAction = CurrentAction;
        // No transition to do at the end of lemming movement
        NextAction = NoneAction.Instance;

        HandleParticleTimer();
        HandleCountDownTimer();
        HandleFastForwardTimer();

        var checkZombies = HandleLemmingAction() &&
                           CheckLevelBoundaries() &&
                           CheckTriggerAreas(false) &&
                           CurrentAction != ExiterAction.Instance &&
                           !State.IsZombie &&
                           LevelScreen.LemmingManager.AnyZombies();

        if (checkZombies)
        {
            LevelScreen.LemmingManager.DoZombieCheck(this);
        }
    }

    public void Simulate(bool checkGadgets)
    {
        if (!IsSimulation)
            throw new InvalidOperationException("Use simulation lemming for simulations!");

        HandleParticleTimer();
        HandleCountDownTimer();
        HandleFastForwardTimer();

        var handleGadgets = HandleLemmingAction() && CheckLevelBoundaries() && checkGadgets;
        if (handleGadgets)
        {
            CheckTriggerAreas(false);
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
    }

    /// <summary>
    /// Handle one frame of the lemming's action, and update data accordingly.
    /// Some actions (e.g. exiter, splatter, etc) may result in the lemming being killed off in some way.
    /// In that case, the method returns false - no extra work is necessary (don't bother checking gadgets, for example).
    /// </summary>
    /// <returns>True if more work needs to be done this frame</returns>
    private bool HandleLemmingAction()
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

        PreviousLevelPosition = LevelPosition;
        PreviousTopLeftPixel = TopLeftPixel;
        PreviousBottomRightPixel = BottomRightPixel;

        var result = CurrentAction.UpdateLemming(this);
        var lemmingBounds = CurrentAction.GetLemmingBounds(this);

        TopLeftPixel = lemmingBounds.GetTopLeftPosition();
        BottomRightPixel = lemmingBounds.GetBottomRightPosition();

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

    private bool CheckTriggerAreas(bool isPostTeleportCheck)
    {
        if (isPostTeleportCheck)
        {
            PreviousLevelPosition = LevelPosition;
        }

        var result = CheckGadgets() && LemmingManager.DoBlockerCheck(this);

        NextAction.TransitionLemmingToAction(this, false);

        return result;
    }

    [SkipLocalsInit]
    private bool CheckGadgets()
    {
        Span<LevelPosition> checkPositions = stackalloc LevelPosition[LemmingMovementHelper.MaxIntermediateCheckPositions];

        // Use first four entries of span to hold level positions.
        // To fetch gadgets, we need to check all gadgets that overlap a certain rectangle.
        // That rectangle is defined as being the minimum bounding box of four level positions:
        // the anchor and foot positions of the current frame, and the anchor and foot
        // positions of the previous frame.
        // Fixes (literal) edge cases when lemmings and gadgets pass chunk position boundaries
        var p = LevelPosition;
        checkPositions[0] = p;
        p = CurrentAction.GetFootPosition(this, p);
        checkPositions[1] = p;
        p = PreviousLevelPosition;
        checkPositions[2] = p;
        p = PreviousAction.GetFootPosition(this, p);
        checkPositions[3] = p;

        var checkPositionsBounds = new LevelRegion(checkPositions[..4]);

        var gadgetManager = LevelScreen.GadgetManager;
        Span<uint> scratchSpaceSpan = stackalloc uint[gadgetManager.ScratchSpaceSize];
        gadgetManager.GetAllItemsNearRegion(scratchSpaceSpan, checkPositionsBounds, out var gadgetSet);

        if (gadgetSet.Count == 0)
            return true;

        // Reuse the above span. LemmingMovementHelper will overwrite existing values
        var movementHelper = new LemmingMovementHelper(this, checkPositions);
        var length = movementHelper.EvaluateCheckPositions();

        return CheckGadgetHitBoxCollisions(in gadgetSet, checkPositions[..length]);
    }

    private bool CheckGadgetHitBoxCollisions(in GadgetEnumerable gadgetEnumerable, ReadOnlySpan<LevelPosition> intermediatePositions)
    {
        foreach (var gadget in gadgetEnumerable)
        {
            foreach (var anchorPosition in intermediatePositions)
            {
                var footPosition = CurrentAction.GetFootPosition(this, anchorPosition);
                if (!gadget.MatchesLemmingAtPosition(this, anchorPosition) &&
                    !gadget.MatchesLemmingAtPosition(this, footPosition))
                    continue;

                var beforeAction = CurrentAction;
                HandleGadgetInteraction(gadget, anchorPosition);
                var afterAction = CurrentAction;

                if (beforeAction == afterAction)
                    continue;

                LevelPosition = anchorPosition;

                var lemmingBounds = afterAction.GetLemmingBounds(this);

                TopLeftPixel = lemmingBounds.GetTopLeftPosition();
                BottomRightPixel = lemmingBounds.GetBottomRightPosition();
                return false;
            }
        }

        return true;
    }

    private void HandleGadgetInteraction(HitBoxGadget gadget, LevelPosition checkPosition)
    {
        // If we're at the end of the check positions and Next action is not None
        // then transition. However, if NextAction is SplatterAction and there's water
        // at the position, the water takes precedence over splatting
        if (NextAction == NoneAction.Instance || checkPosition != LevelPosition ||
            (NextAction == SplatterAction.Instance && gadget.GadgetBehaviour == WaterGadgetBehaviour.Instance))
        {
            gadget.OnLemmingMatch(this);

            return;
        }

        NextAction.TransitionLemmingToAction(this, false);
        if (JumpToHoistAdvance)
        {
            AnimationFrame += 2;
            PhysicsFrame += 2;
            JumpToHoistAdvance = false;
        }

        NextAction = NoneAction.Instance;

        gadget.OnLemmingMatch(this);
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

    public Span<LevelPosition> GetJumperPositions() => _jumperPositionBuffer;

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
        LevelPosition = otherLemming.LevelPosition;
        PreviousLevelPosition = otherLemming.PreviousLevelPosition;

        FacingDirection = otherLemming.FacingDirection;
        Orientation = otherLemming.Orientation;

        PreviousAction = otherLemming.PreviousAction;
        CurrentAction = otherLemming.CurrentAction;
        NextAction = otherLemming.NextAction;

        State.SetRawDataFromOther(otherLemming.State);

        TopLeftPixel = otherLemming.TopLeftPixel;
        BottomRightPixel = otherLemming.BottomRightPixel;
        PreviousTopLeftPixel = otherLemming.PreviousTopLeftPixel;
        PreviousBottomRightPixel = otherLemming.PreviousBottomRightPixel;
    }

    public void SetRawDataFromOther(Team team, uint rawStateData, Orientation orientation, FacingDirection facingDirection)
    {
        State.SetRawDataFromOther(rawStateData);
        State.TeamAffiliation = team;
        Orientation = orientation;
        FacingDirection = facingDirection;
    }

    public void ToSnapshotData(out LemmingSnapshotData lemmingSnapshotData)
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
        LevelPosition = lemmingSnapshotData.LevelPosition;
        PreviousLevelPosition = lemmingSnapshotData.PreviousLevelPosition;
        TopLeftPixel = lemmingSnapshotData.TopLeftPixel;
        BottomRightPixel = lemmingSnapshotData.BottomRightPixel;
        PreviousTopLeftPixel = lemmingSnapshotData.PreviousTopLeftPixel;
        PreviousBottomRightPixel = lemmingSnapshotData.PreviousBottomRightPixel;

        State.SetRawDataFromSnapshotData(in lemmingSnapshotData.StateSnapshotData);

        FacingDirection = FacingDirection.AllItems[lemmingSnapshotData.FacingDirectionId];
        Orientation = Orientation.AllItems[lemmingSnapshotData.OrientationId];

        PreviousAction = LemmingAction.GetActionFromId(lemmingSnapshotData.PreviousActionId);
        CurrentAction = LemmingAction.GetActionFromId(lemmingSnapshotData.CurrentActionId);
        NextAction = LemmingAction.GetActionFromId(lemmingSnapshotData.NextActionId);
        CountDownAction = LemmingAction.GetActionFromId(lemmingSnapshotData.CountDownActionId);

        Renderer.ResetPosition();
    }

    int IIdEquatable<Lemming>.Id => Id;

    public bool Equals(Lemming? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is Lemming other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(Lemming left, Lemming right) => left.Id == right.Id;
    public static bool operator !=(Lemming left, Lemming right) => left.Id != right.Id;

    [InlineArray(JumperAction.JumperPositionCount)]
    public struct JumperPositionBuffer
    {
        private LevelPosition _firstElement;
    }
}