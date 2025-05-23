﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

public interface IHitBoxRegion : IRectangularBounds
{
    bool ContainsPoint(Point levelPosition);
    bool ContainsEitherPoint(Point p1, Point p2);
}