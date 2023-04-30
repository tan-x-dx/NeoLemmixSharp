using Microsoft.Xna.Framework.Graphics;
using System;

namespace NeoLemmixSharp.Rendering;

public abstract class ScreenRenderer : IDisposable
{
    public abstract void RenderScreen(SpriteBatch spriteBatch);

    public abstract void Dispose();
}