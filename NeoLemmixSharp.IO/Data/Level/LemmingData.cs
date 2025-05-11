using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Level;

public sealed class LemmingData
{
    public Point Position { get; set; }
    public uint State { get; set; }
    public Orientation Orientation { get; set; } = Orientation.Down;
    public FacingDirection FacingDirection { get; set; } = FacingDirection.Right;
    public int TeamId { get; set; } = EngineConstants.ClassicTeamId;
    public int InitialLemmingActionId { get; set; } = EngineConstants.WalkerActionId;

    internal LemmingData()
    {
    }
}