namespace NeoLemmixSharp.Engine.LemmingSkills;

public static class LemmingConstants
{
    public const uint MinimumSubstantialAlphaValue = 31;

    /// <summary>
    /// a walker walks one pixel per frame
    /// </summary>
    public const int WalkerStep = 1;
    /// <summary>
    /// a climber climbs up 1 pixel every 2nd frame
    /// </summary>
    public const int ClimberStep = 1;
    /// <summary>
    /// at this height a walker will turn around
    /// </summary>
    public const int WalkerObstacleHeight = 14;
    /// <summary>
    /// check N pixels above the lemming's feet
    /// </summary>
    public const int BasherCheckStep = 12;
    /// <summary>
    /// from this on a basher will become a faller
    /// </summary>
    public const int BasherFallDistance = 6;
    /// <summary>
    /// from this on a miner will become a faller
    /// </summary>
    public const int MinerFallDistance = 4;
    /// <summary>
    /// a faller falls down three pixels per frame
    /// </summary>
    public const int FallerStep = 3;
    /// <summary>
    /// a floater falls down two pixels per frame
    /// </summary>
    public const int FloaterStep = 2;
    /// <summary>
    /// Minimum height for a wall to turn a lemming around
    /// </summary>
    public const int MinimumWallHeight = 7;
    /// <summary>
    /// an ascender moves up two pixels per frame
    /// </summary>
    public const int AscenderStep = 2;
    /// <summary>
    /// if a walker jumps up 3 pixels, it becomes an ascender
    /// </summary>
    public const int AscenderJump = 3;
    /// <summary>
    /// pixels a floater falls before the parachute begins to open
    /// </summary>
    public const int FallDistanceFloat = 32;
    /// <summary>
    /// number of free pixels below needed to convert a lemming to a faller
    /// </summary>
    public const int FallDistanceFall = 4;
    /// <summary>
    /// used as "free below" value to convert most skills into a faller
    /// </summary>
    public const int FallDistanceForceFall = 2 * FallDistanceFall;
    /// <summary>
    /// number of steps a builder can build
    /// </summary>
    public const int StepsMax = 12;
    /// <summary>
    /// number of steps before the warning sound is played
    /// </summary>
    public const int StepsWarning = StepsMax - 3;
}