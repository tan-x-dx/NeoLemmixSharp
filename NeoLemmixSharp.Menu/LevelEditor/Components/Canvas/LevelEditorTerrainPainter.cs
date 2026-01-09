using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.LevelBuilding;
using NeoLemmixSharp.IO.Data.Level;
using System.Runtime.CompilerServices;

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

        var levelDimensions = _levelData.LevelDimensions;

        int x;
        int y = levelDimensions.H + LevelEditorConstants.LevelOuterBoundarySize + 1;
        ref Color color = ref Unsafe.NullRef<Color>();

        for (x = levelDimensions.W + 1; x >= -1; x--)
        {
            color = ref rawColors[new Common.Point(x + LevelEditorConstants.LevelOuterBoundarySize, LevelEditorConstants.LevelOuterBoundarySize - 1)];
            RenderingHelpers.Negate(ref color);

            color = ref rawColors[new Common.Point(x + LevelEditorConstants.LevelOuterBoundarySize, y)];
            RenderingHelpers.Negate(ref color);
        }

        x = levelDimensions.W + LevelEditorConstants.LevelOuterBoundarySize + 1;

        for (y = levelDimensions.H; y >= 0; y--)
        {
            color = ref rawColors[new Common.Point(LevelEditorConstants.LevelOuterBoundarySize - 1, y + LevelEditorConstants.LevelOuterBoundarySize)];
            RenderingHelpers.Negate(ref color);

            color = ref rawColors[new Common.Point(x, y + LevelEditorConstants.LevelOuterBoundarySize)];
            RenderingHelpers.Negate(ref color);
        }

        _terrainPainter.Apply();
    }
}
