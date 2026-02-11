using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Level;

public sealed class LemmingInstanceData : IInstanceData
{
    public Point Position { get; set; }
    public Size Size => new(16, 16);
    public uint State { get; set; }
    public Orientation Orientation { get; set; } = Orientation.Down;
    public FacingDirection FacingDirection { get; set; } = FacingDirection.Right;
    public int TribeId { get; set; } = EngineConstants.ClassicTribeId;
    public int InitialLemmingActionId { get; set; } = LemmingActionConstants.WalkerActionId;

    internal LemmingInstanceData()
    {
    }
}
