using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Util;
using System;
using NeoLemmixSharp.Rendering.Text;

namespace NeoLemmixSharp.Screen;

public abstract class BaseScreen : IDisposable
{
    public IGameWindow GameWindow { get; set; }

    public string ScreenTitle { get; }

    protected BaseScreen(string screenTitle)
    {
        ScreenTitle = screenTitle;
    }

    public abstract void Tick();
    public abstract void OnWindowSizeChanged();

    public abstract void Dispose();

    public abstract ScreenRenderer CreateScreenRenderer(
        SpriteBank spriteBank,
        FontBank fontBank,
        ISprite[] levelSprites);
}