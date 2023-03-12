using System;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Rendering;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace NeoLemmixSharp.Screen;

public abstract class BaseScreen : ITickable, IRenderable, IDisposable
{
    public abstract void Tick(MouseState mouseState);
    public abstract void Render(SpriteBatch spriteBatch);

    public abstract void Dispose();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ITickable.ShouldTick => true;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IRenderable.ShouldRender => true;
}