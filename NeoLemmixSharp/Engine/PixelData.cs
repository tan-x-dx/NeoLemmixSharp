using System;

namespace NeoLemmixSharp.Engine;

public sealed class PixelData
{
    public bool IsSolid;
    public bool IsSteel;

    public readonly int[] GadgetIds = Array.Empty<int>();
}