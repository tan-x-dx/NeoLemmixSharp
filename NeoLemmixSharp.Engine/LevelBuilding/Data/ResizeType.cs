﻿namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

[Flags]
public enum ResizeType
{
    None = 0,
    ResizeHorizontal = 1 << 0,
    ResizeVertical = 1 << 1,
    ResizeBoth = ResizeHorizontal | ResizeVertical,
}