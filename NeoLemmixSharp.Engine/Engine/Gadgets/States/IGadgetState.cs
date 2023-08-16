using NeoLemmixSharp.Engine.Engine.Gadgets.HitBoxes;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.States;

public interface IGadgetState
{
    int AnimationFrame { get; }
    IHitBox HitBox { get; }

    void OnTransitionTo();
    void Tick();
    void OnTransitionFrom();
    void OnLemmingInHitBox(Lemming lemming);
}