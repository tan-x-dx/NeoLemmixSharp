using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Rendering;

namespace NeoLemmixSharp.Menu;

public sealed class MenuScreen : IBaseScreen
{
    public MenuScreenRenderer MenuScreenRenderer { get; }

    public IGameWindow GameWindow { get; set; }
    IScreenRenderer IBaseScreen.ScreenRenderer => MenuScreenRenderer;
    public string ScreenTitle => "NeoLemmixSharp";
    public bool IsDisposed { get; private set; }

    public MenuScreen()
    {
        
    }

    public void Tick()
    {
    }

    public void OnWindowSizeChanged()
    {
    }

    public void Dispose()
    {
    }
}