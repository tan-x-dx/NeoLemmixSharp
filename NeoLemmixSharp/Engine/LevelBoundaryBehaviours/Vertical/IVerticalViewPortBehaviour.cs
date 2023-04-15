﻿namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;

public interface IVerticalViewPortBehaviour
{
    // Raw pixels, one-to-one with game
    int ViewPortY { get; }
    int ViewPortHeight { get; }

    // Stretched to fit the screen
    int ScreenY { get; }
    int ScreenHeight { get; }

    int NumberOfVerticalTilings { get; }

    int NormaliseY(int y);
    void RecalculateVerticalDimensions(int scaleMultiplier, int windowHeight);
    void ScrollVertically(int dy);
}
