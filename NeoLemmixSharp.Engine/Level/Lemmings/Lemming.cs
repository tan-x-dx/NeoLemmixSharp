using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class Lemming : IIdEquatable<Lemming>, IRectangularBounds
{
    private static TerrainManager TerrainManager { get; set; } = null!;
    private static LemmingManager LemmingManager { get; set; } = null!;
    private static GadgetManager GadgetManager { get; set; } = null!;

    public static void SetTerrainManager(TerrainManager terrainManager)
    {
        TerrainManager = terrainManager;
    }

    public static void SetLemmingManager(LemmingManager lemmingManager)
    {
        LemmingManager = lemmingManager;
    }

    public static void SetGadgetManager(GadgetManager gadgetManager)
    {
        GadgetManager = gadgetManager;
    }

    public int Id { get; }

    public bool ConstructivePositionFreeze;
    public bool IsStartingAction;
    public bool PlacedBrick;
    public bool StackLow;
    public bool InitialFall;
    public bool EndOfAnimation;
    public bool LaserHit;
    public bool JumpToHoistAdvance;

    public bool Debug;

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
        var levelPositionPair = CurrentAction.GetLemmingBounds(this);
        TopLeftPixel = levelPositionPair.GetTopLeftPosition();
        BottomRightPixel = levelPositionPair.GetBottomRightPosition();

        PreviousLevelPosition = LevelPosition;
        PreviousTopLeftPixel = TopLeftPixel;
        PreviousBottomRightPixel = BottomRightPixel;
    }

    public void Tick()
    {
        if (Debug)
        {
            ;
        }

        PreviousAction = CurrentAction;
        // No transition to do at the end of lemming movement
        NextAction = NoneAction.Instance;

        _ = HandleLemmingAction() && CheckLevelBoundaries() && CheckTriggerArea(false);
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

            if (CurrentAction.IsOneTimeAction)
            {
                EndOfAnimation = true;
            }
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
        var footPixel = TerrainManager.PixelTypeAtPosition(FootPosition);
        var headPixel = TerrainManager.PixelTypeAtPosition(HeadPosition);

        if (!footPixel.IsVoid() || !headPixel.IsVoid())
            return true;

        LemmingManager.RemoveLemming(this);
        return false;
    }

    private bool CheckTriggerArea(bool isPostTeleportCheck)
    {
        var needShiftPosition = false;

        var currentAnchorPixel = LevelPosition;
        var currentFootPixel = Orientation.MoveUp(currentAnchorPixel, 1);

        LevelPosition previousAnchorPixel, previousFootPixel;

        if (isPostTeleportCheck)
        {
            PreviousLevelPosition = currentAnchorPixel;
            previousAnchorPixel = currentAnchorPixel;
            previousFootPixel = currentFootPixel;
        }
        else
        {
            previousAnchorPixel = PreviousLevelPosition;
            previousFootPixel = Orientation.MoveUp(previousAnchorPixel, 1);
        }

        var checkRegion = new LevelPositionPair(currentAnchorPixel, currentFootPixel, previousAnchorPixel, previousFootPixel);

        CheckGadgets(checkRegion);

        NextAction.TransitionLemmingToAction(this, false);

        return true;
    }

    private void CheckGadgets(LevelPositionPair levelRegion)
    {
        var gadgetSet = GadgetManager.GetAllItemsNearRegion(levelRegion);
        var blockerSet = LemmingManager.LemmingIsBlocking(this)
            ? LargeSimpleSet<Lemming>.Empty
            : LemmingManager.GetAllBlockersNearLemming(levelRegion);

        if (gadgetSet.Count == 0 &&
            blockerSet.Count == 0)
            return;

        Span<LevelPosition> checkPositions = stackalloc LevelPosition[LemmingMovementHelper.MaxIntermediateCheckPositions];
        var movementHelper = new LemmingMovementHelper(this, checkPositions);
        var length = movementHelper.EvaluateCheckPositions();

        for (var i = 0; i < length; i++)
        {
            var anchorPosition = checkPositions[i];
            var footPosition = Orientation.MoveUp(anchorPosition, 1);

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
                return;
            }

            foreach (var blocker in blockerSet)
            {
                if (BlockerAction.BlockerMatches(blocker, this, anchorPosition, footPosition))
                {

                }
            }
        }
    }

    private void HandleGadgetInteraction(HitBoxGadget gadget, LevelPosition checkPosition)
    {
        // Transition if we are at the end position and need to do one
        // Except if we try to splat and there is water at the lemming position - then let this take precedence.
        if (NextAction != NoneAction.Instance &&
            checkPosition == LevelPosition &&
            (NextAction != SplatterAction.Instance || gadget.Type != WaterGadgetType.Instance))
        {
            NextAction.TransitionLemmingToAction(this, false);
            if (JumpToHoistAdvance)
            {
                AnimationFrame += 2;
                PhysicsFrame += 2;
            }

            NextAction = NoneAction.Instance;
            JumpToHoistAdvance = false;
        }

        gadget.OnLemmingMatch(this, checkPosition);
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