﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Rendering.Level.Viewport;

namespace NeoLemmixSharp.Rendering.Level.Ui;

public sealed class ControlPanelSpriteBank : IDisposable
{
    private readonly Dictionary<string, Texture2D> _textureLookup;

    public ControlPanelSpriteBank(Dictionary<string, Texture2D> textureLookup, LevelCursorSprite levelCursorSprite)
    {
        _textureLookup = textureLookup;
        LevelCursorSprite = levelCursorSprite;
    }

    public LevelCursorSprite LevelCursorSprite { get; }

    public void Dispose()
    {

    }

    public Texture2D GetTexture(string textureName)
    {
        return _textureLookup[textureName];
    }
}