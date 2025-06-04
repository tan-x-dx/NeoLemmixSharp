using Microsoft.Xna.Framework;
using NeoLemmixSharp.IO.Data.Style.Theme;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

public sealed class GadgetLayerColorData
{
    public required int StateIndex { get; init; }
    public required int LayerIndex { get; init; }

    public Color SpecificColor { get; }

    public int TribeId { get; }
    public TribeSpriteLayerColorType SpriteLayerColorType { get; }

    public bool UsesSpecificColor => TribeId == -1;

    public GadgetLayerColorData(Color specificColor)
    {
        SpecificColor = specificColor;
        TribeId = -1;
        SpriteLayerColorType = default;
    }

    public GadgetLayerColorData(int tribeId, TribeSpriteLayerColorType spriteLayerColorType)
    {
        SpecificColor = default;
        TribeId = tribeId;
        SpriteLayerColorType = spriteLayerColorType;
    }
}
