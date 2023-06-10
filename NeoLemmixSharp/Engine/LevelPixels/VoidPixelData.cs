using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LevelGadgets;

namespace NeoLemmixSharp.Engine.LevelPixels;

public sealed class VoidPixelData : IPixelData
{
    public bool IsVoid => true;
    public bool IsSolidToLemming(Lemming lemming) => false;
    public bool IsIndestructibleToLemming(Lemming lemming) => false;

    bool IPixelData.HasGadgetThatMatchesTypeAndOrientation(GadgetType gadgetType, Orientation orientation) => false;
    bool IPixelData.ErasePixel() => false;
    bool IPixelData.SetSolid() => false;
}