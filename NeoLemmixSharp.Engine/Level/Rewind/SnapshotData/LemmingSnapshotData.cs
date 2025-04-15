using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;

public readonly struct LemmingSnapshotData
{
    public readonly int Id;
    public readonly Lemming.JumperPositionBuffer JumperPositionBuffer = new();

    public readonly bool ConstructivePositionFreeze;
    public readonly bool IsStartingAction;
    public readonly bool PlacedBrick;
    public readonly bool StackLow;

    public readonly bool InitialFall;
    public readonly bool EndOfAnimation;
    public readonly bool LaserHit;
    public readonly bool JumpToHoistAdvance;

    public readonly int AnimationFrame;
    public readonly int PhysicsFrame;
    public readonly int AscenderProgress;
    public readonly int NumberOfBricksLeft;
    public readonly int DisarmingFrames;
    public readonly int DistanceFallen;
    public readonly int JumpProgress;
    public readonly int TrueDistanceFallen;
    public readonly int LaserRemainTime;

    public readonly int FastForwardTime;
    public readonly int CountDownTimer;
    public readonly int ParticleTimer;

    public readonly Point DehoistPin;
    public readonly Point LaserHitLevelPosition;
    public readonly Point LevelPosition;
    public readonly Point PreviousLevelPosition;

    public readonly RectangularRegion CurrentBounds;

    public readonly LemmingStateSnapshotData StateSnapshotData;

    public readonly FacingDirection FacingDirection;
    public readonly Orientation Orientation;

    public readonly int PreviousActionId;
    public readonly int CurrentActionId;
    public readonly int NextActionId;
    public readonly int CountDownActionId;

    public LemmingSnapshotData(Lemming lemming)
    {
        Id = lemming.Id;

        Span<Point> jumperPositionSource = lemming.GetJumperPositions();
        Span<Point> jumperPositionDest = JumperPositionBuffer;
        jumperPositionSource.CopyTo(jumperPositionDest);

        ConstructivePositionFreeze = lemming.ConstructivePositionFreeze;
        IsStartingAction = lemming.IsStartingAction;
        PlacedBrick = lemming.PlacedBrick;
        StackLow = lemming.StackLow;

        InitialFall = lemming.InitialFall;
        EndOfAnimation = lemming.EndOfAnimation;
        LaserHit = lemming.LaserHit;
        JumpToHoistAdvance = lemming.JumpToHoistAdvance;

        AnimationFrame = lemming.AnimationFrame;
        PhysicsFrame = lemming.PhysicsFrame;
        AscenderProgress = lemming.AscenderProgress;
        NumberOfBricksLeft = lemming.NumberOfBricksLeft;
        DisarmingFrames = lemming.DisarmingFrames;
        DistanceFallen = lemming.DistanceFallen;
        JumpProgress = lemming.JumpProgress;
        TrueDistanceFallen = lemming.TrueDistanceFallen;
        LaserRemainTime = lemming.LaserRemainTime;

        FastForwardTime = lemming.FastForwardTime;
        CountDownTimer = lemming.CountDownTimer;
        ParticleTimer = lemming.ParticleTimer;

        DehoistPin = lemming.DehoistPin;
        LaserHitLevelPosition = lemming.LaserHitLevelPosition;
        LevelPosition = lemming.AnchorPosition;
        PreviousLevelPosition = lemming.PreviousLevelPosition;
        CurrentBounds = lemming.CurrentBounds;

        lemming.State.WriteToSnapshotData(out StateSnapshotData);

        FacingDirection = lemming.FacingDirection;
        Orientation = lemming.Orientation;

        PreviousActionId = lemming.PreviousAction.Id;
        CurrentActionId = lemming.CurrentAction.Id;
        NextActionId = lemming.NextAction.Id;
        CountDownActionId = lemming.CountDownAction.Id;
    }
}
