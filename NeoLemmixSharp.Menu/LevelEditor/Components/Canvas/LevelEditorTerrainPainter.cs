using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.LevelBuilding;
using NeoLemmixSharp.IO.Data.Level;
using Point = NeoLemmixSharp.Common.Point;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed class LevelEditorTerrainPainter
{
    private readonly LevelData _levelData;
    private readonly TerrainPainter _terrainPainter;

    public LevelEditorTerrainPainter(
        LevelData levelData,
        RenderTarget2D canvasTexture)
    {
        _levelData = levelData;
        _terrainPainter = new TerrainPainter(levelData, canvasTexture, LevelEditorConstants.RenderOffset);
    }

    public void RepaintTerrain()
    {
        var rawColors = _terrainPainter.TerrainColors;
        new Span<Color>(rawColors.Array).Fill(Color.Black);
        _terrainPainter.PaintTerrain();

        DrawLevelSpaceBoundary();

        _terrainPainter.Apply();
    }

    private void DrawLevelSpaceBoundary()
    {
        var rawColors = _terrainPainter.TerrainColors;
        var levelDimensions = _levelData.LevelDimensions;
        int x;
        int y = levelDimensions.H + LevelEditorConstants.LevelOuterBoundarySize + 1;
        Point p;

        for (x = levelDimensions.W + 1; x >= -1; x--)
        {
            p = new Point(x + LevelEditorConstants.LevelOuterBoundarySize, LevelEditorConstants.LevelOuterBoundarySize - 1);
            RenderingHelpers.Negate(ref rawColors[p]);

            p = new Point(x + LevelEditorConstants.LevelOuterBoundarySize, y);
            RenderingHelpers.Negate(ref rawColors[p]);
        }

        x = levelDimensions.W + LevelEditorConstants.LevelOuterBoundarySize + 1;

        for (y = levelDimensions.H; y >= 0; y--)
        {
            p = new Point(LevelEditorConstants.LevelOuterBoundarySize - 1, y + LevelEditorConstants.LevelOuterBoundarySize);
            RenderingHelpers.Negate(ref rawColors[p]);

            p = new Point(x, y + LevelEditorConstants.LevelOuterBoundarySize);
            RenderingHelpers.Negate(ref rawColors[p]);
        }
    }
}
