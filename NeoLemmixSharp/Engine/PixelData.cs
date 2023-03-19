using System;

namespace NeoLemmixSharp.Engine;

public sealed class PixelData
{
    public bool IsVoid;
    public bool IsSolid;
    public bool IsSteel;

    public readonly int[] GadgetIds = Array.Empty<int>();
}