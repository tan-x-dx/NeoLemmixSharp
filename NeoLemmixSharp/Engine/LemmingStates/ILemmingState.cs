namespace NeoLemmixSharp.Engine.LemmingStates;

public interface ILemmingState
{
    int LemmingStateId { get; }

    void UpdateLemming(Lemming lemming);
}