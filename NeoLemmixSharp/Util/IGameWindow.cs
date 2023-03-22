namespace NeoLemmixSharp.Util;

public interface IGameWindow
{
    int WindowWidth { get; }
    int WindowHeight { get; }

    bool IsActive { get; }
    bool IsFullScreen { get; }
    bool IsFastForwards { get; }

    void ToggleFullScreen();
    void SetFastForwards(bool fastForwards);
    void Escape();
}