using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Rendering.LevelRendering;
using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.Rendering;

public sealed class SpriteBank : IDisposable
{
    private readonly Dictionary<string, LemmingActionSpriteBundle> _actionSpriteBundleLookup;
    private readonly Dictionary<string, Texture2D> _textureLookup;

    public TerrainSprite TerrainSprite { get; }
    public Texture2D AnchorTexture { get; init; }
    public Texture2D WhitePixelTexture { get; init; }
    public LevelCursorSprite LevelCursorSprite { get; init; }

    public IReadOnlyDictionary<string, LemmingActionSpriteBundle> LemmingActionSpriteBundleLookup => _actionSpriteBundleLookup;
    public IReadOnlyDictionary<string, Texture2D> TextureLookup => _textureLookup;

    public SpriteBank(
        Dictionary<string, LemmingActionSpriteBundle> actionSpriteBundleLookup,
        Dictionary<string, Texture2D> textureLookup,
        TerrainSprite terrainSprite)
    {
        _actionSpriteBundleLookup = actionSpriteBundleLookup;
        _textureLookup = textureLookup;
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

        foreach (var texture in _textureLookup.Values)
        {
            texture.Dispose();
        }

        _actionSpriteBundleLookup.Clear();
        _textureLookup.Clear();

        TerrainSprite.Dispose();
        AnchorTexture.Dispose();
    }
}