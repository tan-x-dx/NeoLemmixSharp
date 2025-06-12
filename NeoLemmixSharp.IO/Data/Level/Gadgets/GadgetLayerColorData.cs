using Microsoft.Xna.Framework;
using NeoLemmixSharp.IO.Data.Style.Theme;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

[StructLayout(LayoutKind.Explicit, Size = 4 * sizeof(int))]
public readonly struct GadgetLayerColorData
{
    [FieldOffset(0 * sizeof(int))] public readonly int StateIndex;
    [FieldOffset(1 * sizeof(int))] public readonly int LayerIndex;

    [FieldOffset(2 * sizeof(int))] public readonly Color SpecificColor;
    [FieldOffset(3 * sizeof(int))] private readonly int _dummy;

    [FieldOffset(2 * sizeof(int))] public readonly int TribeId;
    [FieldOffset(3 * sizeof(int))] public readonly TribeSpriteLayerColorType SpriteLayerColorType;

    public bool UsesSpecificColor => _dummy == -1;

    public GadgetLayerColorData(
        int stateIndex,
        int layerIndex,
        Color specificColor)
    {
        StateIndex = stateIndex;
        LayerIndex = layerIndex;
        SpecificColor = specificColor;
        _dummy = -1;
    }

    public GadgetLayerColorData(
        int stateIndex,
        int layerIndex,
        int tribeId,
        TribeSpriteLayerColorType spriteLayerColorType)
    {
        StateIndex = stateIndex;
        LayerIndex = layerIndex;
        TribeId = tribeId;
        SpriteLayerColorType = spriteLayerColorType;
    }
}
