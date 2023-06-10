using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LevelGadgets;

namespace NeoLemmixSharp.Engine.LevelPixels;

public interface IPixelData
{
    bool IsVoid { get; }

    bool IsSolidToLemming(Lemming lemming);
    bool IsIndestructibleToLemming(Lemming lemming);

    bool HasGadgetThatMatchesTypeAndOrientation(GadgetType gadgetType, Orientation orientation);

    bool ErasePixel();
    bool SetSolid();
}