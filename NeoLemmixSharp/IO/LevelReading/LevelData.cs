using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Terrain;
using System.Collections.Generic;

namespace NeoLemmixSharp.IO.LevelReading;

public sealed class LevelData
{
    private RenderTarget2D? _levelRenderTarget;

    public string LevelTitle { get; set; }
    public string LevelAuthor { get; set; }
    public int LevelWidth { get; set; }
    public int LevelHeight { get; set; }
    public int LevelStartX { get; set; }
    public int LevelStartY { get; set; }

    public List<ITickable> LevelObjects { get; } = new();
    public List<IRenderable> LevelSprites { get; } = new();

    public RenderTarget2D LevelTexture(GraphicsDevice graphicsDevice)
    {
        if (_levelRenderTarget != null)
            return _levelRenderTarget;
        
        _levelRenderTarget = new RenderTarget2D(graphicsDevice, LevelWidth, LevelHeight);

        return _levelRenderTarget;
    }

    public TerrainSprite TerrainSprite => new(LevelWidth, LevelHeight, _levelRenderTarget!);
}