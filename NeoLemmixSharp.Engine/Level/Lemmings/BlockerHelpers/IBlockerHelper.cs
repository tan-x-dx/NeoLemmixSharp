namespace NeoLemmixSharp.Engine.Level.Lemmings.BlockerHelpers;

public interface IBlockerHelper
{
    void RegisterBlocker(Lemming lemming);
    void UpdateBlockerPosition(Lemming lemming);
    bool LemmingIsBlocking(Lemming lemming);
    void DeregisterBlocker(Lemming lemming);

    bool CanAssignBlocker(Lemming lemming);
    void CheckBlockers(Lemming lemming);
}