using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.Activation;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.HitBoxes;

namespace NeoLemmixSharp.Engine.Level.Gadgets.States;

public sealed class GadgetState
{
    private readonly int _numberOfAnimationFrames;

    public int AnimationFrame { get; private set; }

    public IActivationBehaviour ActivationBehaviour { get; }
    public IHitBoxBehaviour HitBoxBehaviour { get; }

    public GadgetState(
        int numberOfAnimationFrames,
        IActivationBehaviour activationBehaviour,
        IHitBoxBehaviour hitBoxBehaviour)
    {
        _numberOfAnimationFrames = numberOfAnimationFrames;
        ActivationBehaviour = activationBehaviour;
        HitBoxBehaviour = hitBoxBehaviour;
    }

    public void OnTransitionTo()
    {
        AnimationFrame = 0;
    }

    public void Tick()
    {
        if (AnimationFrame < _numberOfAnimationFrames)
        {
            AnimationFrame++;
        }
        else
        {
            AnimationFrame = 0;
        }
    }

    public void OnTransitionFrom()
    {
    }
}