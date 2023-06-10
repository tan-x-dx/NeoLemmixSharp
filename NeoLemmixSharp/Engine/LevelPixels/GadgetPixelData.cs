using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LevelGadgets;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelPixels;

public sealed class GadgetPixelData : IPixelData
{
    private readonly List<Gadget> _gadgets = new();

    public bool IsSolid { get; private set; }
    public bool IsSteel { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IPixelData.IsVoid => false;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IPixelData.CanAcceptGadgets => true;

    public GadgetPixelData(bool isSolid, bool isSteel)
    {
        IsSolid = isSolid || isSteel;
        IsSteel = isSteel;
    }

    public void AddGadget(Gadget gadget)
    {
        _gadgets.Add(gadget);
    }

    public void RemoveGadget(Gadget gadget)
    {
        _gadgets.Remove(gadget);
    }

    public bool IsSolidToLemming(Lemming lemming)
    {
        if (IsSolid)
            return true;

        for (var i = 0; i < _gadgets.Count; i++)
        {
            if (_gadgets[i].IsSolidToLemming(lemming))
                return true;
        }

        return false;
    }

    public bool IsIndestructibleToLemming(Lemming lemming)
    {
        if (IsSteel)
            return true;

        for (var i = 0; i < _gadgets.Count; i++)
        {
            if (_gadgets[i].IsIndestructibleToLemming(lemming))
                return true;
        }

        return false;
    }

    public bool HasGadgetThatMatchesTypeAndOrientation(GadgetType gadgetType, Orientation orientation)
    {
        for (var i = 0; i < _gadgets.Count; i++)
        {
            if (_gadgets[i].MatchesTypeAndOrientation(gadgetType, orientation))
                return true;
        }

        return false;
    }

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