﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Terrain;
using NeoLemmixSharp.Screen;

namespace NeoLemmixSharp.Engine;

public sealed class LevelScreen : BaseScreen
{
    public static LevelScreen? CurrentLevel { get; private set; }

    public ITickable[] LevelObjects { private get; init; }
    public IRenderable[] LevelSprites { private get; init; }
    public LevelTerrain Terrain { get; }

    public NeoLemmixViewPort Viewport { get; init; }
    public TerrainSprite TerrainSprite { get; init; }

    public int Width => Terrain.Width;
    public int Height => Terrain.Height;

    public LevelScreen(
        LevelData levelData,
        LevelTerrain terrain)
        : base(levelData.LevelTitle)
    {
        Terrain = terrain;

        CurrentLevel = this;
    }

    public override void Tick(MouseState mouseState)
    {
        Viewport.Tick(mouseState);

      /*  for (var i = 0; i < LevelObjects.Length; i++)
        {
            if (LevelObjects[i].ShouldTick)
            {
                LevelObjects[i].Tick(mouseState);
            }
        }*/
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