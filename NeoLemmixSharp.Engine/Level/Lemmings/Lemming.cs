using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetInteractionTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class Lemming : IIdEquatable<Lemming>, IRectangularBounds
{
    public static Lemming SimulationLemming { get; } = new();

    private LevelPosition[]? _jumperPositions;

    public readonly int Id;

    private readonly bool _isSimulation;
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

    public LevelPosition DehoistPin;
    public LevelPosition LaserHitLevelPosition;
    public LevelPosition LevelPosition;
    public LevelPosition PreviousLevelPosition;

    public LemmingState State { get; }

    public FacingDirection FacingDirection { get; private set; }
    public Orientation Orientation { get; private set; }

    public LemmingAction PreviousAction { get; private set; }
    public LemmingAction CurrentAction { get; private set; }
    public LemmingAction NextAction { get; private set; }
    public LemmingAction CountDownAction { get; private set; }

    public LemmingRenderer Renderer { get; private set; }
    public LevelPosition TopLeftPixel { get; private set; }
    public LevelPosition BottomRightPixel { get; private set; }
    public LevelPosition PreviousTopLeftPixel { get; private set; }
    public LevelPosition PreviousBottomRightPixel { get; private set; }

    public bool IsFastForward => FastForwardTime > 0 || State.IsPermanentFastForwards;

    public LevelPosition HeadPosition => Orientation.MoveUp(LevelPosition, 6);
    public LevelPosition FootPosition => Orientation.MoveUp(LevelPosition, 1);

    public Lemming(
        int id,
        Orientation orientation,
        FacingDirection facingDirection,
        LemmingAction currentAction)
    {
        Id = id;
        _isSimulation = false;
        Orientation = orientation;
        FacingDirection = facingDirection;
        CurrentAction = currentAction;
        State = new LemmingState(this, Team.AllItems[0]);
        Renderer = new LemmingRenderer(this);
    }

    private Lemming()
    {
        Id = -1;
        _isSimulation = true;
        Orientation = DownOrientation.Instance;
        FacingDirection = FacingDirection.RightInstance;
        CurrentAction = NoneAction.Instance;
        State = new LemmingState(this, Team.AllItems[0]);
    }

    public void Initialise()
    {
        State.IsActive = true;
        var levelPositionPair = CurrentAction.GetLemmingBounds(this);
        TopLeftPixel = levelPositionPair.GetTopLeftPosition();
        BottomRightPixel = levelPositionPair.GetBottomRightPosition();

        PreviousLevelPosition = LevelPosition;
        PreviousTopLeftPixel = TopLeftPixel;
        PreviousBottomRightPixel = BottomRightPixel;

        var initialAction = CurrentAction;
        if (initialAction == NoneAction.Instance)
        {
            initialAction = WalkerAction.Instance;
        }

        initialAction.TransitionLemmingToAction(this, false);
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
        if (!_isSimulation)
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
        PhysicsFrame++;

        if (PhysicsFrame >= CurrentAction.NumberOfAnimationFrames)
        {
            if (CurrentAction == FloaterAction.Instance ||
                CurrentAction == GliderAction.Instance)
            {
                PhysicsFrame = 9;
            }
            else
            {
                PhysicsFrame = 0;
            }

            EndOfAnimation = CurrentAction.IsOneTimeAction;
        }

        AnimationFrame = PhysicsFrame;
        // AnimationFrame is usually identical to PhysicsFrame
        // Exceptions occur in JumperAction, for example

        PreviousLevelPosition = LevelPosition;
        PreviousTopLeftPixel = TopLeftPixel;
        PreviousBottomRightPixel = BottomRightPixel;

        var result = CurrentAction.UpdateLemming(this);
        var levelPositionPair = CurrentAction.GetLemmingBounds(this);

        TopLeftPixel = levelPositionPair.GetTopLeftPosition();
        BottomRightPixel = levelPositionPair.GetBottomRightPosition();

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

    private bool CheckGadgets()
    {
        var checkPositionsBounds = new LevelPositionPair(LevelPosition, PreviousLevelPosition);

        var gadgetSet = LevelScreen.GadgetManager.GetAllItemsNearRegion(checkPositionsBounds);

        if (gadgetSet.Count == 0)
            return true;

        Span<LevelPosition> checkPositions = stackalloc LevelPosition[LemmingMovementHelper.MaxIntermediateCheckPositions];
        var movementHelper = new LemmingMovementHelper(this, checkPositions);
        var length = movementHelper.EvaluateCheckPositions();

        ReadOnlySpan<LevelPosition> checkPositionsReadOnly = checkPositions[..length];

        foreach (var anchorPosition in checkPositionsReadOnly)
        {
            var footPosition = Orientation.MoveWithoutNormalization(anchorPosition, 0, 1);

            foreach (var gadget in gadgetSet)
            {
                if (!gadget.MatchesLemmingAtPosition(this, anchorPosition) &&
                    !gadget.MatchesLemmingAtPosition(this, footPosition))
                    continue;

                var beforeAction = CurrentAction;
                HandleGadgetInteraction(gadget, anchorPosition);
                var afterAction = CurrentAction;

                if (beforeAction == afterAction)
                    continue;

                LevelPosition = anchorPosition;

                var levelPositionPair = CurrentAction.GetLemmingBounds(this);

                TopLeftPixel = levelPositionPair.GetTopLeftPosition();
                BottomRightPixel = levelPositionPair.GetBottomRightPosition();
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
            (NextAction == SplatterAction.Instance && gadget.GadgetSubType == WaterGadgetInteractionType.Instance))
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

    public void SetRawData(Team team, uint rawStateData, Orientation orientation, FacingDirection facingDirection)
    {
        State.SetRawData(team, rawStateData);
        Orientation = orientation;
        FacingDirection = facingDirection;
    }

    public void OnRemoval(LemmingRemovalReason removalReason)
    {
        CurrentAction = NoneAction.Instance;
        Renderer.UpdateLemmingState(removalReason == LemmingRemovalReason.DeathExplode);
    }

    public Span<LevelPosition> GetJumperPositions()
    {
        _jumperPositions ??= new LevelPosition[JumperAction.JumperPositionCount];

        return new Span<LevelPosition>(_jumperPositions);
    }

    public void SetRawData(Lemming otherLemming)
    {
        _jumperPositions = otherLemming._jumperPositions;

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

        Renderer = otherLemming.Renderer;
        State.SetRawData(otherLemming.State);

        TopLeftPixel = otherLemming.TopLeftPixel;
        BottomRightPixel = otherLemming.BottomRightPixel;
        PreviousTopLeftPixel = otherLemming.PreviousTopLeftPixel;
        PreviousBottomRightPixel = otherLemming.PreviousBottomRightPixel;
    }

    int IIdEquatable<Lemming>.Id => Id;

    public bool Equals(Lemming? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is Lemming other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(Lemming left, Lemming right) => left.Id == right.Id;
    public static bool operator !=(Lemming left, Lemming right) => left.Id != right.Id;
}