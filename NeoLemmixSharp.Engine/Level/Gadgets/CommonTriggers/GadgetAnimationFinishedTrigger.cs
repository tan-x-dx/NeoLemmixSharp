using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.GadgetAnimation;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

public sealed class GadgetAnimationFinishedTrigger : GadgetTrigger
{
    private readonly AnimationLayerBehaviour _animationLayerBehaviour;

    public GadgetAnimationFinishedTrigger(AnimationLayerBehaviour animationLayerBehaviour)
        : base(GadgetTriggerType.GadgetAnimationFinished)
    {
        _animationLayerBehaviour = animationLayerBehaviour;
    }

    public override void DetectTrigger()
    {
        if (_animationLayerBehaviour.AnimationFinished)
        {
            DetermineTrigger(true);
            TriggerBehaviours();
        }
        else
        {
            DetermineTrigger(false);
        }
        MarkAsEvaluated();
    }
}
