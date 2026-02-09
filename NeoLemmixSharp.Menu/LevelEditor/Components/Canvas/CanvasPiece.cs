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

    public RectangularRegion GetBounds() => new(_anchorPosition, InstanceData.Size);

    public Color GetOutlineColor() => InstanceData switch
    {
        GadgetInstanceData => Color.Green,
        LemmingInstanceData => Color.Green,
        SketchInstanceData => Color.Green,
        TerrainInstanceData => Color.Yellow,

        _ => Color.White
    };

    public CanvasPiece(IInstanceData instanceData)
    {
        InstanceData = instanceData;
        FixPosition();
    }

    public void SetPosition(Point position)
    {
        InstanceData.Position = position;
        _anchorPosition = position;
    }

    public void Move(Point offsetPosition)
    {
        InstanceData.Position = _anchorPosition + offsetPosition;
    }

    public void FixPosition()
    {
        _anchorPosition = InstanceData.Position;
    }

    public bool ContainsPoint(Point point) => GetBounds().Contains(point);

    public bool OverlapsRegion(RectangularRegion rectangularRegion) => GetBounds().Overlaps(rectangularRegion);
}
