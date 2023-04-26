using System;
using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LemmingActions;

namespace NeoLemmixSharp.Engine;

public sealed class PixelData
{
    public bool IsVoid;
    public bool IsSolid;
    public bool IsSteel;

    public readonly int[] GadgetIds = Array.Empty<int>();

    public bool IsIndestructible(
        Orientation orientation,
        FacingDirection facingDirection,
        LemmingAction lemmingAction)
    {
        if (IsSteel)
            return true;


        return false;
    }
}