using NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Engine.Gadgets.HitBoxes;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.States;

public sealed class GenericGadgetState : IGadgetState
{
    private readonly Dictionary<InputType, List<IBehaviour>> _behaviours;

    private readonly int _numberOfAnimationFrames;

    public GenericGadgetState(
        Dictionary<InputType, List<IBehaviour>> behaviours,
        int numberOfAnimationFrames)
    {
        _behaviours = behaviours;
        _numberOfAnimationFrames = numberOfAnimationFrames;
    }

    public int AnimationFrame { get; private set; }
    public IHitBox HitBox { get; }

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

    public void OnLemmingInHitBox(Lemming lemming)
    {

    }
}