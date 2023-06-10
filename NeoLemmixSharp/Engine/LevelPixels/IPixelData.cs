using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LevelGadgets;

namespace NeoLemmixSharp.Engine.LevelPixels;

public interface IPixelData
{
    bool IsSolid { get; }
    bool IsSteel { get; }
    bool IsVoid { get; }

    bool CanAcceptGadgets { get; }
    void AddGadget(Gadget gadget);
    void RemoveGadget(Gadget gadget);

    bool IsSolidToLemming(Lemming lemming);
    bool IsIndestructibleToLemming(Lemming lemming);

    bool HasGadgetThatMatchesTypeAndOrientation(GadgetType gadgetType, Orientation orientation);

    bool ErasePixel();
    bool SetSolid();
}