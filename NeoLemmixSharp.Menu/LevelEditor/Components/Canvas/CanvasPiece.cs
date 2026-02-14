using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed class CanvasPiece
{
    public int PieceOrder { get; set; }

    public IInstanceData InstanceData { get; }

    private Point _anchorPosition;
    public Point Position => InstanceData.Position;
    public Size Size => InstanceData.Size;

    public RectangularRegion GetBounds() => InstanceData.GetBounds(_anchorPosition);

    public Color GetOutlineColor() => InstanceData switch
    {
        GadgetInstanceData => Color.SpringGreen,
        LemmingInstanceData => Color.SpringGreen,
        SketchInstanceData => Color.SpringGreen,
        TerrainInstanceData => Color.Yellow,

        _ => Color.White
    };

    public CanvasPiece(IInstanceData instanceData)
    {
        InstanceData = instanceData;
        _anchorPosition = instanceData.Position;
    }

    public void SetPosition(Point position)
    {
        InstanceData.Position = position;
        _anchorPosition = position;
    }

    public void Move(Point offsetPosition, RectangularRegion clampBounds)
    {
        InstanceData.Position = _anchorPosition + offsetPosition;
        ClampPosition(clampBounds);
    }

    public void FixPosition(RectangularRegion clampBounds)
    {
        _anchorPosition = InstanceData.Position;
        ClampPosition(clampBounds);
    }

    private void ClampPosition(RectangularRegion clampBounds)
    {
        var piecePosition = InstanceData.Position;
        var pieceSize = InstanceData.Size;
        var x = Math.Clamp(piecePosition.X, clampBounds.X, clampBounds.X + clampBounds.W - pieceSize.W);
        var y = Math.Clamp(piecePosition.Y, clampBounds.Y, clampBounds.Y + clampBounds.H - pieceSize.H);

        InstanceData.Position = new Point(x, y);
    }

    public bool ContainsPoint(Point point) => GetBounds().Contains(point);
}
