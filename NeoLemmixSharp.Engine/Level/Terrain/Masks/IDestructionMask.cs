using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public interface IDestructionMask
{
    string Name { get; }

    [Pure]
    bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection);
}