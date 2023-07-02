using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Rendering;
using System;
using NeoLemmixSharp.Common.Rendering;

namespace NeoLemmixSharp.Screen;

public abstract class BaseScreen : IDisposable
{
    public abstract ScreenRenderer ScreenRenderer { get; }
    public bool IsDisposed { get; protected set; }

    public IGameWindow GameWindow { get; set; }

    public string ScreenTitle { get; protected init; }

    public abstract void Tick();
    public abstract void OnWindowSizeChanged();

    public abstract void Dispose();
}