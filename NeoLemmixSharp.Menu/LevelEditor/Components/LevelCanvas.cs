using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Ui.Components;
using Color = Microsoft.Xna.Framework.Color;
using Point = NeoLemmixSharp.Common.Point;
using Size = NeoLemmixSharp.Common.Size;

namespace NeoLemmixSharp.Menu.LevelEditor.Components;

public sealed class LevelCanvas : Component
{
    private const int MinZoom = 1;
    private const int MaxZoom = 12;

    private const int NegativeSpaceBoundary = 128;

    private const int MinCanvasBorderThickness = 16;
    private const uint CanvasBorderColourValue = 0xff696969;

    private int _zoom;
    private int _cameraX;
    private int _cameraY;
    private int _cameraWidth;
    private int _cameraHeight;

    public LevelData LevelData { get; set; }

    public LevelCanvas(int x, int y, int width, int height) : base(x, y, width, height, string.Empty)
    {
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        var p = Position;
        var s = Dimensions;

        spriteBatch.FillRect(Helpers.CreateRectangle(p, s), new Color(CanvasBorderColourValue));

        p += new Point(MinCanvasBorderThickness, MinCanvasBorderThickness);
        s = new Size(s.W - (MinCanvasBorderThickness * 2), s.H - (MinCanvasBorderThickness * 2));

        spriteBatch.FillRect(Helpers.CreateRectangle(p, s), Color.Black);
    }

    public void Zoom(int scrollDelta)
    {
        if (scrollDelta == 0)
            return;

        var currentZoom = Math.Clamp(_zoom + scrollDelta, MinZoom, MaxZoom);

        _zoom = currentZoom;

        RecalculateCameraValues();
    }

    public void Scroll(int dx, int dy)
    {

    }

    private void RecalculateCameraValues()
    {
    }
}
