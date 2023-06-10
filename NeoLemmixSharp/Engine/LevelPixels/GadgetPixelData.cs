using NeoLemmixSharp.Engine.LevelGadgets;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelPixels;

public sealed class GadgetPixelData : IPixelData
{
    private bool _isSolid;
    private readonly bool _isSteel;

    private readonly List<IGadget> _gadgets = new();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IPixelData.IsVoid => false;

    public GadgetPixelData(bool isSolid, bool isSteel)
    {
        _isSolid = isSolid || isSteel;
        _isSteel = isSteel;
    }

    public void AddGadget(IGadget gadget)
    {
        _gadgets.Add(gadget);
    }

    public bool IsSolidToLemming(Lemming lemming)
    {
        if (_isSolid)
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
        if (_isSteel)
            return true;

        for (var i = 0; i < _gadgets.Count; i++)
        {
            if (_gadgets[i].IsIndestructibleToLemming(lemming))
                return true;
        }

        return false;
    }

    public void CheckGadgets(Lemming lemming)
    {
        for (var i = 0; i < _gadgets.Count; i++)
        {
            if (_gadgets[i].InteractsWithLemming(lemming))
                return;
        }
    }

    public bool ErasePixel()
    {
        // Some clever bool logic here.
        var previouslyWasSolid = _isSolid;
        _isSolid = _isSteel;

        return previouslyWasSolid != _isSolid;
    }

    public bool SetSolid()
    {
        var previouslyWasSolid = _isSolid;
        _isSolid = true;

        return previouslyWasSolid != _isSolid;
    }
}