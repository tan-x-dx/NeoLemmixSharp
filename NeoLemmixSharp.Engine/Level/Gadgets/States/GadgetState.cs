using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.Activation;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.Movement;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.Resizing;

namespace NeoLemmixSharp.Engine.Level.Gadgets.States;

public sealed class GadgetState
{
    private readonly int _numberOfAnimationFrames;

    public int AnimationFrame { get; private set; }

    public IActivationBehaviour ActivationBehaviour { get; }
    public IMovementBehaviour MovementBehaviour { get; }
    public IResizeBehaviour ResizeBehaviour { get; }
    public IHitBoxBehaviour HitBoxBehaviour { get; }

    public GadgetState(
        int numberOfAnimationFrames,
        IActivationBehaviour activationBehaviour,
        IMovementBehaviour movementBehaviour,
        IResizeBehaviour resizeBehaviour,
        IHitBoxBehaviour hitBoxBehaviour)
    {
        _numberOfAnimationFrames = numberOfAnimationFrames;
        ActivationBehaviour = activationBehaviour;
        MovementBehaviour = movementBehaviour;
        ResizeBehaviour = resizeBehaviour;
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