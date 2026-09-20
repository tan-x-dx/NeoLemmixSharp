using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering.Shaders;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Terrain;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed class LevelEditorTerrainPainter
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly RenderTarget2D _canvasTexture;
    private readonly LevelData _levelData;

    public LevelEditorTerrainPainter(
        GraphicsDevice graphicsDevice,
        RenderTarget2D canvasTexture,
        LevelData levelData)
    {
        _graphicsDevice = graphicsDevice;
        _canvasTexture = canvasTexture;
        _levelData = levelData;
    }

    public void RepaintTerrain(SpriteBatch spriteBatch)
    {
        _graphicsDevice.SetRenderTarget(_canvasTexture);
        _graphicsDevice.Clear(Color.Transparent);

        if (_levelData.AllTerrainInstanceData.Count > 0)
        {
            spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp);

            var previousTerrainRenderType = _levelData.AllTerrainInstanceData[0].RenderType;

            foreach (TerrainInstanceData terrainPiece in _levelData.AllTerrainInstanceData)
            {
                DrawTerrainPiece(spriteBatch, terrainPiece, previousTerrainRenderType);
                previousTerrainRenderType = terrainPiece.RenderType;
            }

            spriteBatch.End();
        }
        _graphicsDevice.SetRenderTarget(null);
    }

    private static void SwitchToShaderContext(SpriteBatch spriteBatch, TerrainRenderType terrainRenderType)
    {
        spriteBatch.End();

        switch (terrainRenderType)
        {
            case TerrainRenderType.Erase:
                spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp, effect: ShaderBank.GreyScaleTintShader);
                break;
            case TerrainRenderType.NoOverwrite:
                spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp);
                break;
            default:
                spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp);
                break;
        }
    }

    private static void DrawTerrainPiece(
        SpriteBatch spriteBatch,
        TerrainInstanceData currentTerrainPiece,
        TerrainRenderType previousRenderType)
    {
        if (currentTerrainPiece.RenderType != previousRenderType)
            SwitchToShaderContext(spriteBatch, currentTerrainPiece.RenderType);

        var terrainTexture = GetTerrainTexture(currentTerrainPiece);
        var sourceRectangle = terrainTexture.Bounds;

        var dht = currentTerrainPiece.DihedralTransformation;
        var destinationRectangle = Helpers.CreateRectangle(currentTerrainPiece.Position + LevelEditorConstants.RenderOffset, currentTerrainPiece.Size);

        var rotationAngle = dht.Orientation.GetRotationAngle();
        var spriteEffects = dht.FacingDirection.AsSpriteEffects();

        spriteBatch.Draw(
            terrainTexture,
            destinationRectangle,
            sourceRectangle,
            currentTerrainPiece.Tint,
            rotationAngle,
            Vector2.Zero,
            spriteEffects,
            1.0f);
    }

    /*
    private void DrawLevelSpaceBoundary()
    {
        var rawColors = _terrainPainter.TerrainColors;
        var levelDimensions = _levelData.LevelDimensions;
        int x;
        int y = levelDimensions.H + LevelEditorConstants.LevelOuterBoundarySize;
        Point p;

        for (x = levelDimensions.W; x >= -1; x--)
        {
            p = new Point(x + LevelEditorConstants.LevelOuterBoundarySize, LevelEditorConstants.LevelOuterBoundarySize - 1);
            RenderingHelpers.Negate(ref rawColors[p]);

            p = new Point(x + LevelEditorConstants.LevelOuterBoundarySize, y);
            RenderingHelpers.Negate(ref rawColors[p]);
        }

        x = levelDimensions.W + LevelEditorConstants.LevelOuterBoundarySize;

        for (y = levelDimensions.H - 1; y >= 0; y--)
        {
            p = new Point(LevelEditorConstants.LevelOuterBoundarySize - 1, y + LevelEditorConstants.LevelOuterBoundarySize);
            RenderingHelpers.Negate(ref rawColors[p]);

            p = new Point(x, y + LevelEditorConstants.LevelOuterBoundarySize);
            RenderingHelpers.Negate(ref rawColors[p]);
        }
    }
    */

    private static Texture2D GetTerrainTexture(TerrainInstanceData terrainData)
    {
        return TextureCache.GetOrLoadTexture(terrainData.StyleIdentifier, terrainData.PieceIdentifier, TextureType.TerrainSprite);
    }
}
