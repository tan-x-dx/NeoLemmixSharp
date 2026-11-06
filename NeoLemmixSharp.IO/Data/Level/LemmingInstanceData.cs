using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Level;

public sealed class LemmingInstanceData : IInstanceData
{
    public Point Position { get; set; }
    public Size Size { get; private set; }

    public PackedBytes32 State { get; set; }
    public DihedralTransformation DihedralTransformation
    {
        get;
        set
        {
            field = value;
            UpdateSize();
        }
    }
    public int TribeId { get; set; } = EngineConstants.ClassicTribeId;
    public LemmingActionType InitialLemmingActionType
    {
        get;
        set
        {
            field = value;
            UpdateSize();
        }
    }

    internal LemmingInstanceData()
    {
        InitialLemmingActionType = LemmingActionType.WalkerAction;
        UpdateSize();
    }

    private void UpdateSize()
    {
        var dht = DihedralTransformation;
        var actionBounds = LemmingActionBounds.GetBounds(InitialLemmingActionType);

        Size = dht.Transform(actionBounds.Size);
    }

    RectangularRegion IInstanceData.GetBounds(Point anchorPosition)
    {
        var basicBounds = LemmingActionBounds.GetBounds(InitialLemmingActionType);
        var dht = DihedralTransformation;
        var transformedBounds = dht.Transform(basicBounds);
        transformedBounds = transformedBounds.Translate(anchorPosition);
        return transformedBounds;
    }
}
