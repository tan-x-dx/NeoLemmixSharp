namespace NeoLemmixSharp.Util;

public interface IGameWindow
{
    bool IsFullScreen { get; }
    bool IsFastForwards { get; }

    void ToggleFullScreen();
    void SetFastForwards(bool fastForwards);
    void Escape();
}