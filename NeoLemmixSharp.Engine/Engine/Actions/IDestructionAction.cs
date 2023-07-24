using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public interface IDestructionAction
{
    bool CanDestroyPixel(PixelType pixelType, Orientation orientation, FacingDirection facingDirection);
}