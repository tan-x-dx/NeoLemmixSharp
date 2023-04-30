using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Rendering.LevelRendering;
using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.Rendering;

public sealed class SpriteBank : IDisposable
{
    private readonly Dictionary<string, LemmingActionSpriteBundle> _actionSpriteBundleLookup;

    public TerrainSprite TerrainSprite { get; }
    public Texture2D BoxTexture { get; init; }
    public Texture2D AnchorTexture { get; init; }
    public LevelCursorSprite LevelCursorSprite { get; init; }

    public SpriteBank(
        Dictionary<string, LemmingActionSpriteBundle> actionSpriteBundleLookup,
        TerrainSprite terrainSprite)
    {
        _actionSpriteBundleLookup = actionSpriteBundleLookup;
        TerrainSprite = terrainSprite;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        TerrainSprite.Render(spriteBatch);
    }

    public void Dispose()
    {
        foreach (var actionSpriteBundle in _actionSpriteBundleLookup.Values)
        {
            actionSpriteBundle.Dispose();
        }

        _actionSpriteBundleLookup.Clear();

        TerrainSprite.Dispose();
        BoxTexture.Dispose();
        AnchorTexture.Dispose();
    }
}