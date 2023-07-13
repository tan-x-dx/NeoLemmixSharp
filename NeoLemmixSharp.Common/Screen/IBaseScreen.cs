using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Screen;

public interface IBaseScreen : IDisposable
{
    IScreenRenderer ScreenRenderer { get; }
    bool IsDisposed { get; protected set; }

    IGameWindow GameWindow { get; set; }

    string ScreenTitle { get; protected init; }

    void Tick();
    void OnWindowSizeChanged();
}