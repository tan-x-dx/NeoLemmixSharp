namespace NeoLemmixSharp.Engine.Engine.Gadgets.States;

public interface IGadgetState
{
    int AnimationFrame { get; }


    void OnTransitionTo();
    void Tick();
    void OnTransitionFrom();
}