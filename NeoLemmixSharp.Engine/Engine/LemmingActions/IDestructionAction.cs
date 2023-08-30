using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Engine.LemmingActions;

public interface IDestructionAction
{
    [Pure]
    bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection);
}