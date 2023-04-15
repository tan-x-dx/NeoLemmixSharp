﻿using System;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;

public sealed class HorizontalWrapBehaviour : IHorizontalViewPortBehaviour
{
    private readonly int _width;

    public int ViewPortX { get; private set; }
    public int ViewPortWidth { get; private set; }
    public int ScreenX => 0;
    public int ScreenWidth { get; private set; }
    public int NumberOfHorizontalTilings { get; private set; }

    public HorizontalWrapBehaviour(int width)
    {
        _width = width;
    }

    public int NormaliseX(int x)
    {
        if (x < 0)
            return x + _width;

        if (x >= _width)
            return x - _width;

        return x;
    }

    public void RecalculateHorizontalDimensions(int scaleMultiplier, int windowWidth)
    {
        ViewPortWidth = windowWidth / scaleMultiplier;

        if (ViewPortWidth < _width)
        {
            ScreenWidth = ViewPortWidth * scaleMultiplier;
        }
        else
        {
            ScreenWidth = _width * scaleMultiplier;
            ViewPortWidth = _width;
        }

        ScrollHorizontally(0);
    }

    public void ScrollHorizontally(int dx)
    {
        ViewPortX += dx;
        if (ViewPortX < 0)
        {
            ViewPortX += _width;
        }
        else if (ViewPortX >= _width)
        {
            ViewPortX -= _width;
        }
    }
    
    public int GetNumberOfHorizontalRepeats()
    {
        return Math.Clamp(1 + (ScreenWidth + ScreenX) / _width, 1, 5);
    }
}