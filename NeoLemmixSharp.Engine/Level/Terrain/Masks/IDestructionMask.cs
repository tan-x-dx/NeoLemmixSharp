using NeoLemmixSharp.Common;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Terrain.Masks;

public interface IDestructionMask
{
    [Pure]
    bool CanDestroyPixel(DihedralTransformation dht, PixelType pixelType);
}