using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;

namespace NeoLemmixSharp.Engine.LevelIo.Data;

public sealed class LemmingData
{
    public Point Position { get; set; }
    public uint State { get; set; }

    public Orientation Orientation { get; set; } = Orientation.Down;
    public FacingDirection FacingDirection { get; set; } = FacingDirection.Right;
    public int TeamId { get; set; } = EngineConstants.ClassicTeamId;
    public LemmingAction InitialLemmingAction { get; set; } = WalkerAction.Instance;
}