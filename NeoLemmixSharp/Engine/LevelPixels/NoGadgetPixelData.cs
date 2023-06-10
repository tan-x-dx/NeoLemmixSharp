namespace NeoLemmixSharp.Engine.LevelPixels;

public sealed class NoGadgetPixelData : IPixelData
{
    private bool _isSolid;
    private readonly bool _isSteel;

    public NoGadgetPixelData(bool isSolid, bool isSteel)
    {
        _isSolid = isSolid;
        _isSteel = isSteel;
    }

    public bool IsVoid => false;
    public bool IsSolidToLemming(Lemming lemming) => _isSolid;

    public bool IsIndestructibleToLemming(Lemming lemming) => _isSteel;

    void IPixelData.CheckGadgets(Lemming lemming)
    {
    }

    public bool ErasePixel()
    {
        if (_isSteel)
            return false;

        var previouslyWasSolid = _isSolid;
        _isSolid = false;

        return previouslyWasSolid != _isSolid;
    }

    public bool SetSolid()
    {
        if (_isSteel)
            return false;

        var previouslyWasSolid = _isSolid;
        _isSolid = true;

        return previouslyWasSolid != _isSolid;
    }
}