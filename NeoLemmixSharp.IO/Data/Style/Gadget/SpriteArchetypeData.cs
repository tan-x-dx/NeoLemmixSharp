using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Theme;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public sealed class SpriteArchetypeData
{
    public required Size BaseSpriteSize { get; init; }
    public required int NumberOfLayers { get; init; }
    public required int MaxNumberOfFrames { get; init; }

    public required StateSpriteArchetypeData[] SpriteArchetypeDataForStates { get; init; }
}

public readonly struct StateSpriteArchetypeData
{
    public required AnimationLayerArchetypeData[] AnimationData { get; init; }
}

public sealed class AnimationLayerArchetypeData
{
    public required AnimationLayerParameters AnimationLayerParameters { get; init; }
    public required NineSliceData NineSliceData { get; init; }
    public required TribeSpriteLayerColorType ColorType { get; init; }
    public required int InitialFrame { get; init; }
    public required int NextGadgetState { get; init; }
}
