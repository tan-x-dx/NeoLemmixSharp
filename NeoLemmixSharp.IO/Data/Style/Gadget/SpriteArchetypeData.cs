using NeoLemmixSharp.IO.Data.Style.Theme;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public sealed class AnimationLayerArchetypeData
{
    public required AnimationLayerParameters AnimationLayerParameters { get; init; }
    public required NineSliceData NineSliceData { get; init; }
    public required TribeSpriteLayerColorType ColorType { get; init; }
    public required int InitialFrame { get; init; }
    public required int NextGadgetState { get; init; }

    internal AnimationLayerArchetypeData()
    {
    }
}
