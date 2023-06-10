using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelPixels;

public sealed class NoGadgetPixelData : IPixelData
{
    private bool _isSolid;
    private readonly bool _isSteel;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IPixelData.IsVoid => false;

    public NoGadgetPixelData(bool isSolid, bool isSteel)
    {
        _isSolid = isSolid || isSteel;
        _isSteel = isSteel;
    }

    public bool IsSolidToLemming(Lemming lemming) => _isSolid;

    public bool IsIndestructibleToLemming(Lemming lemming) => _isSteel;

    void IPixelData.CheckGadgets(Lemming lemming)
    {
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