namespace NeoLemmixSharp.Engine.LevelPixels;

public sealed class VoidPixelData : IPixelData
{
    public bool IsVoid => true;
    public bool IsSolidToLemming(Lemming lemming) => false;
    public bool IsIndestructibleToLemming(Lemming lemming) => false;

    void IPixelData.CheckGadgets(Lemming lemming)
    {
    }
    bool IPixelData.ErasePixel() => false;
    bool IPixelData.SetSolid() => false;
}