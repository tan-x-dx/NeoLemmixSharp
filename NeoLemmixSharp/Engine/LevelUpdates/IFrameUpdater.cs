namespace NeoLemmixSharp.Engine.LevelUpdates;

public interface IFrameUpdater
{
    void UpdateLemming(Lemming lemming);

    void Update();
}