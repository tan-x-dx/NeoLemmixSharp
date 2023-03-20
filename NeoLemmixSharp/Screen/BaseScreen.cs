using System;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine;
using NeoLemmixSharp.Rendering;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Screen;

public abstract class BaseScreen : ITickable, IRenderable, IDisposable
{
    public IGameWindow GameWindow { protected get; set; }

    public string ScreenTitle { get; }

    protected BaseScreen(string screenTitle)
    {
        ScreenTitle = screenTitle;
    }

    public abstract void Tick(MouseState mouseState);
    public abstract void Render(SpriteBatch spriteBatch);

    public abstract void Dispose();

    public abstract void KeyInput(KeyboardState keyboardState);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ITickable.ShouldTick => true;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IRenderable.ShouldRender => true;
}