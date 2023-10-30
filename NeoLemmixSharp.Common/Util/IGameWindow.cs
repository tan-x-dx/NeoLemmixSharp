using NeoLemmixSharp.Common.Screen;

namespace NeoLemmixSharp.Common.Util;

public interface IGameWindow
{
    int WindowWidth { get; }
    int WindowHeight { get; }

    bool IsActive { get; }
    bool IsFullScreen { get; }

    void SetScreen(IBaseScreen screen);
    void ToggleFullScreen();
    void ToggleBorderless();
    void Escape();
}