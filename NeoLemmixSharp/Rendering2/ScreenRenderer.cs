using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Util;
using System;

namespace NeoLemmixSharp.Rendering2;

public abstract class ScreenRenderer : IDisposable
{
    public bool IsDisposed { get; protected set; }

    public IGameWindow GameWindow { get; set; }

    public abstract void RenderScreen(SpriteBatch spriteBatch);
    public abstract void OnWindowSizeChanged(int windowWidth, int windowHeight);

    public abstract void Dispose();
}