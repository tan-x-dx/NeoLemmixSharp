namespace NeoLemmixSharp.Engine.LevelUpdates;

public interface ILevelUpdater
{
    bool IsFastForwards { get; }
    void ToggleFastForwards();
    void UpdateLemming(Lemming lemming);
    void Update();
}