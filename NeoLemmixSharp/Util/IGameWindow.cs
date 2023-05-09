﻿namespace NeoLemmixSharp.Util;

public interface IGameWindow
{
    int WindowWidth { get; }
    int WindowHeight { get; }

    bool IsActive { get; }
    bool IsFullScreen { get; }

    void ToggleFullScreen();
    void Escape();
}