using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class Lemming : IIdEquatable<Lemming>, IRectangularBounds
{
    public int Id { get; }

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
    public int TrueDistanceFallen;
    public int LaserRemainTime;

    public int FastForwardTime = 0;

    public bool IsFastForward => FastForwardTime > 0;

    public LevelPosition DehoistPin;
    public LevelPosition LaserHitLevelPosition;
    public LevelPosition LevelPosition;
    public LevelPosition PreviousLevelPosition;

    public LevelPosition HeadPosition => Orientation.MoveUp(LevelPosition, 6);
    public LevelPosition FootPosition => Orientation.MoveUp(LevelPosition, 1);

    public FacingDirection FacingDirection { get; private set; }
    public Orientation Orientation { get; private set; }

    public LemmingAction PreviousAction { get; private set; } = NoneAction.Instance;
    public LemmingAction CurrentAction { get; private set; }
    public LemmingAction NextAction { get; private set; } = NoneAction.Instance;

    public LemmingRenderer Renderer { get; private set; } = null!;
    public LemmingState State { get; }

    public LevelPosition TopLeftPixel { get; private set; }
    public LevelPosition BottomRightPixel { get; private set; }
    public LevelPosition PreviousTopLeftPixel { get; private set; }
    public LevelPosition PreviousBottomRightPixel { get; private set; }

    public Lemming(
        int id,
        Orientation? orientation = null,
        FacingDirection? facingDirection = null,
        LemmingAction? currentAction = null)
    {
        Id = id;
        Orientation = orientation ?? DownOrientation.Instance;
        FacingDirection = facingDirection ?? RightFacingDirection.Instance;
        CurrentAction = currentAction ?? WalkerAction.Instance;
        State = new LemmingState(this, Team.AllItems[0]);
    }

    public void SetRenderer(LemmingRenderer lemmingRenderer)
    {
        Renderer = lemmingRenderer;
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
    }

    public void Tick()
    {
        PreviousAction = CurrentAction;
        // No transition to do at the end of lemming movement
        NextAction = NoneAction.Instance;

        var shouldContinue = HandleLemmingAction() && CheckLevelBoundaries() && CheckTriggerAreas(false);

        if (!shouldContinue ||
            CurrentAction == ExiterAction.Instance ||
            State.IsZombie)
            return;

        Global.LemmingManager.DoZombieCheck(this);
    }

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
        var terrainManager = Global.TerrainManager;
        var footPixel = terrainManager.PixelTypeAtPosition(FootPosition);
        var headPixel = terrainManager.PixelTypeAtPosition(HeadPosition);

        if (!footPixel.IsVoid() || !headPixel.IsVoid())
            return true;

        Global.LemmingManager.RemoveLemming(this);
        return false;
    }

    private bool CheckTriggerAreas(bool isPostTeleportCheck)
    {
        if (isPostTeleportCheck)
        {
            PreviousLevelPosition = LevelPosition;
        }

        var result = CheckGadgets() &&
                     CheckBlockers();

        NextAction.TransitionLemmingToAction(this, false);

        return result;
    }

    private bool CheckGadgets()
    {
        var checkPositionsBounds = new LevelPositionPair(LevelPosition, PreviousLevelPosition);

        var gadgetSet = Global.GadgetManager.GetAllItemsNearRegion(checkPositionsBounds);

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
        if (NextAction != NoneAction.Instance && checkPosition == LevelPosition &&
            (NextAction != SplatterAction.Instance || gadget.SubType != WaterGadgetType.Instance))
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

        gadget.OnLemmingMatch(this);
    }

    /// <summary>
    /// Check for blockers, but not for miners removing terrain,
    /// see http://www.lemmingsforums.net/index.php?topic=2710.0.
    /// Also not for Jumpers, as this is handled by the JumperAction
    /// </summary>
    private bool CheckBlockers()
    {
        if (CurrentAction == JumperAction.Instance ||
            (CurrentAction == MinerAction.Instance && (PhysicsFrame == 1 || PhysicsFrame == 2)))
            return true;

        Global.LemmingManager.DoBlockerCheck(this);
        return true;
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

    public void OnInitialization()
    {
        Renderer.UpdateLemmingState(true);
    }

    public void OnRemoval()
    {
        CurrentAction = NoneAction.Instance;
        Renderer.UpdateLemmingState(false);
    }

    public bool Equals(Lemming? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is Lemming other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(Lemming left, Lemming right) => left.Id == right.Id;
    public static bool operator !=(Lemming left, Lemming right) => left.Id != right.Id;
}