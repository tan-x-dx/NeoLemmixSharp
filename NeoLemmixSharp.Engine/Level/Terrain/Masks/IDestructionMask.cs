﻿using NeoLemmixSharp.Common;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public interface IDestructionMask
{
    [Pure]
    bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection);
}