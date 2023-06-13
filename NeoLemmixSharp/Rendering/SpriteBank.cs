using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Rendering.LevelRendering;
using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.Rendering;

public sealed class SpriteBank : IDisposable
{
    private readonly Dictionary<string, LemmingActionSpriteBundle> _actionSpriteBundleLookup;
    private readonly Dictionary<string, Texture2D> _textureLookup;
    private readonly Dictionary<string, ISprite> _spriteLookup;

    public TerrainSprite TerrainSprite { get; }

    public SpriteBank(
        Dictionary<string, LemmingActionSpriteBundle> actionSpriteBundleLookup,
        Dictionary<string, Texture2D> textureLookup,
        Dictionary<string, ISprite> spriteLookup,
        TerrainSprite terrainSprite)
    {
        _actionSpriteBundleLookup = actionSpriteBundleLookup;
        _textureLookup = textureLookup;
        _spriteLookup = spriteLookup;

        TerrainSprite = terrainSprite;
    }

    public Texture2D GetTexture(string textureName)
    {
        return _textureLookup[textureName];
    }

    public T GetSprite<T>(string textureName)
        where T : ISprite
    {
        return (T)_spriteLookup[textureName];
    }

    public LemmingActionSpriteBundle GetLemmingActionSpriteBundle(string spriteName)
    {
        return _actionSpriteBundleLookup[spriteName];
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

        foreach (var sprite in _spriteLookup.Values)
        {
            sprite.Dispose();
        }

        _actionSpriteBundleLookup.Clear();
        _textureLookup.Clear();
        _spriteLookup.Clear();

        TerrainSprite.Dispose();
    }
}