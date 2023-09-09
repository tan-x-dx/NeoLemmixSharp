using System.Diagnostics.Contracts;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public interface IDestructionMask
{
    [Pure]
    bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection);
}