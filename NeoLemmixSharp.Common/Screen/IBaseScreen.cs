using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Screen;

public interface IBaseScreen : IDisposable
{
    IScreenRenderer ScreenRenderer { get; }
    bool IsDisposed { get; }

    IGameWindow GameWindow { get; set; }

    string ScreenTitle { get; }

    void Tick();
    void OnWindowSizeChanged();
}