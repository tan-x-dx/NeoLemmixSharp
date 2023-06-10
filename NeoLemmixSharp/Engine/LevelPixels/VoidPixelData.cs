using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LevelGadgets;
using System;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelPixels;

public sealed class VoidPixelData : IPixelData
{
    public bool IsVoid => true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IPixelData.IsSolid => false;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IPixelData.IsSteel => false;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IPixelData.CanAcceptGadgets => false;

    void IPixelData.AddGadget(Gadget gadget)
    {
        throw new InvalidOperationException("Cannot add gadget to void pixel");
    }

    void IPixelData.RemoveGadget(Gadget gadget)
    {
        throw new InvalidOperationException("Cannot remove gadget from void pixel");
    }

    public bool IsSolidToLemming(Lemming lemming) => false;
    public bool IsIndestructibleToLemming(Lemming lemming) => false;

    bool IPixelData.HasGadgetThatMatchesTypeAndOrientation(GadgetType gadgetType, Orientation orientation) => false;
    bool IPixelData.ErasePixel() => false;
    bool IPixelData.SetSolid() => false;
}