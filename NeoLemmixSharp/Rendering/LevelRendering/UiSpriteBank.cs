using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.Rendering.LevelRendering;

public sealed class UiSpriteBank : IDisposable
{
    private readonly Dictionary<string, Texture2D> _textureLookup;
    private readonly Dictionary<string, ISprite> _spriteLookup;

    public Texture2D GetTexture(string textureName)
    {
        return _textureLookup[textureName];
    }

    public T GetSprite<T>(string textureName)
        where T : ISprite
    {
        return (T)_spriteLookup[textureName];
    }

    public void Dispose()
    {
    }
}