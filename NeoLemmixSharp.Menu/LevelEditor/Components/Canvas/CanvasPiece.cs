using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed class CanvasPiece
{
    public int PieceOrder { get; set; }
    public IInstanceData InstanceData { get; }

    public Point Position => InstanceData.Position;

    public CanvasPiece(IInstanceData instanceData)
    {
        InstanceData = instanceData;
    }

    public void SetPosition(int x, int y)
    {
        InstanceData.Position = new Point(x, y);
    }

    public void SetPosition(Point position)
    {
        InstanceData.Position = position;
    }
}
