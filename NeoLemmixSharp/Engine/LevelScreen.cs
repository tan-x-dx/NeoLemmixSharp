using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Terrain;
using NeoLemmixSharp.Screen;
using System.Collections.Generic;
using NeoLemmixSharp.IO.LevelReading;

namespace NeoLemmixSharp.Engine;

public sealed class LevelScreen : BaseScreen
{
    public static LevelScreen? CurrentLevel { get; private set; }

    public ITickable[] LevelObjects { private get; init; }
    public IRenderable[] LevelSprites { private get; init; }
    private readonly IList<bool> _terrain;
    public ArrayWrapper2D<bool> WrappedTerrain { get; }

    public NeoLemmixViewPort Viewport { get; init; }
    public TerrainSprite TerrainSprite { get; init; }

    public int Width { get; }
    public int Height { get; }

    public LevelScreen(
        string title,
        int width, 
        int height,
        IList<bool> terrain)
        : base(title)
    {
        Width = width;
        Height = height;
        _terrain = terrain;
        WrappedTerrain = new ArrayWrapper2D<bool>(width, height, terrain);
        CurrentLevel = this;
    }

    public override void Tick(MouseState mouseState)
    {
        Viewport.Tick(mouseState);

        for (var i = 0; i < LevelObjects.Length; i++)
        {
            if (LevelObjects[i].ShouldTick)
            {
                LevelObjects[i].Tick(mouseState);
            }
        }
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        TerrainSprite.Render(spriteBatch);

        for (var i = 0; i < LevelSprites.Length; i++)
        {
            if (LevelSprites[i].ShouldRender)
            {
                LevelSprites[i].Render(spriteBatch);
            }
        }
    }

    public override void Dispose()
    {
    }
}