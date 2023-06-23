namespace NeoLemmixSharp.Engine.Updates;

public interface IFrameUpdater
{
    void UpdateLemming(Lemming lemming);

    void Update();
}