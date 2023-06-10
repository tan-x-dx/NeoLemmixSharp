using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LevelGadgets;
using System;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelPixels;

public sealed class NoGadgetPixelData : IPixelData
{
    public bool IsSolid { get; private set; }
    public bool IsSteel { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IPixelData.IsVoid => false;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IPixelData.CanAcceptGadgets => false;

    public NoGadgetPixelData(bool isSolid, bool isSteel)
    {
        IsSolid = isSolid || isSteel;
        IsSteel = isSteel;
    }

    void IPixelData.AddGadget(Gadget gadget)
    {
        throw new InvalidOperationException("Cannot add gadget to no-gadget pixel");
    }

    void IPixelData.RemoveGadget(Gadget gadget)
    {
        throw new InvalidOperationException("Cannot remove gadget from no-gadget pixel");
    }

    public bool IsSolidToLemming(Lemming lemming) => IsSolid;

    public bool IsIndestructibleToLemming(Lemming lemming) => IsSteel;
    public bool HasGadgetThatMatchesTypeAndOrientation(GadgetType gadgetType, Orientation orientation) => false;

    public bool ErasePixel()
    {
        // Some clever bool logic here.
        var previouslyWasSolid = IsSolid;
        IsSolid = IsSteel;

        return previouslyWasSolid != IsSolid;
    }

    public bool SetSolid()
    {
        var previouslyWasSolid = IsSolid;
        IsSolid = true;

        return previouslyWasSolid != IsSolid;
    }
}