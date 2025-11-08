using NeoLemmixSharp.Common;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

[StructLayout(LayoutKind.Sequential, Size = SizeOfLemmingDataInBytes)]
public struct LemmingData
{
    /// <summary>
    /// The size of this struct is explicitly defined to be 192 bytes.
    /// The total size of all members is 188 bytes. We add on an extra
    /// 4 bytes to this size to help with data snapshotting:
    /// The Span.Copy() method works well with chunks that are multiples
    /// of 64 bytes. 3 * 64 == 192.
    /// </summary>
    public const int SizeOfLemmingDataInBytes = 192;

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
    public uint CountDownTimer;
    public int ParticleTimer;

    public LemmingData()
    {
    }
}
