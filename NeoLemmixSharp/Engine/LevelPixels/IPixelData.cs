namespace NeoLemmixSharp.Engine.LevelPixels;

public interface IPixelData
{
    bool IsVoid { get; }

    bool IsSolidToLemming(Lemming lemming);
    bool IsIndestructibleToLemming(Lemming lemming);

    void CheckGadgets(Lemming lemming);

    bool ErasePixel();
    bool SetSolid();
}