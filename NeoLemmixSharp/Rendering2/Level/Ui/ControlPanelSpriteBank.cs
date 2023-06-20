using System;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Rendering2.Level.ViewportSprites;

namespace NeoLemmixSharp.Rendering2.Level.Ui;

public sealed class ControlPanelSpriteBank : IDisposable
{
    public LevelCursorSprite LevelCursorSprite { get; }

    public void Dispose()
    {

    }

    public Texture2D GetTexture(string whitePixel)
    {
        throw new NotImplementedException();
    }

    public IControlPanelRenderer GetControlPanelRenderer()
    {
        throw new NotImplementedException();
    }
}