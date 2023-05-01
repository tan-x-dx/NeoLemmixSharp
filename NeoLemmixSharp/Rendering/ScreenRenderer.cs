using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Util;
using System;

namespace NeoLemmixSharp.Rendering;

public abstract class ScreenRenderer : IDisposable
{
    public IGameWindow GameWindow { get; set; }

    public abstract void RenderScreen(SpriteBatch spriteBatch);

    public abstract void Dispose();
}