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
    public LemmingActionType InitialLemmingActionType { get; set; } = LemmingActionType.WalkerAction;

    internal LemmingInstanceData()
    {
    }

    RectangularRegion IInstanceData.GetBounds(Point anchorPosition)
    {
        var basicBounds = LemmingActionBounds.GetBounds(InitialLemmingActionType);
        var dht = new DihedralTransformation(Orientation, FacingDirection);
        var transformedBounds = dht.Transform(basicBounds);
        transformedBounds = transformedBounds.Translate(anchorPosition);
        return transformedBounds;
    }
}
