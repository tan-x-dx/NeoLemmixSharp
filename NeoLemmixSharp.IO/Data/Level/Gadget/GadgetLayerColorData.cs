using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Enums;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Data.Level.Gadget;

[SkipLocalsInit]
[StructLayout(LayoutKind.Explicit, Size = 2 * sizeof(int))]
public readonly struct GadgetLayerColorData
{
    [FieldOffset(0 * sizeof(int))] public readonly Color SpecificColor;
    [FieldOffset(1 * sizeof(int))] private readonly int _dummy;

    [FieldOffset(0 * sizeof(int))] public readonly int TribeId;
    [FieldOffset(1 * sizeof(int))] public readonly TribeSpriteLayerColorType SpriteLayerColorType;

    public bool UsesSpecificColor => _dummy == -1;

    public GadgetLayerColorData(
        Color specificColor)
    {
        SpecificColor = specificColor;
        _dummy = -1;
    }

    public GadgetLayerColorData(
        int tribeId,
        TribeSpriteLayerColorType spriteLayerColorType)
    {
        TribeId = tribeId;
        SpriteLayerColorType = spriteLayerColorType;
    }
}
