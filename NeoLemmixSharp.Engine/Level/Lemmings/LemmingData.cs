using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public struct LemmingData
{
    public Orientation Orientation = Orientation.Down;
    public FacingDirection FacingDirection = FacingDirection.Right;

    public int TribeId;
    public uint State;

    public RectangularRegion CurrentBounds;

    public int PreviousActionId = LemmingActionConstants.NoneActionId;
    public int CurrentActionId = LemmingActionConstants.NoneActionId;
    public int NextActionId = LemmingActionConstants.NoneActionId;
    public int CountDownActionId = LemmingActionConstants.NoneActionId;

    public JumperPositionBuffer JumperPositionBuffer = new();

    public Point DehoistPin = new(-1, -1);
    public Point LaserHitLevelPosition = new(-1, -1);
    public Point AnchorPosition = new(-1, -1);
    public Point PreviousAnchorPosition = new(-1, -1);

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

    public LemmingData()
    {
    }
}
