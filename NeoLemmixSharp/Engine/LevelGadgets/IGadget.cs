namespace NeoLemmixSharp.Engine.LevelGadgets;

public interface IGadget
{
    bool IsSolidToLemming(Lemming lemming);
    bool IsIndestructibleToLemming(Lemming lemming);
    bool InteractsWithLemming(Lemming lemming);
}