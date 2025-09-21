using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.GadgetAnimation;

public sealed class AnimationLayerBehaviour : GadgetBehaviour
{
    private readonly int _layer;
    private readonly int _minFrame;
    private readonly int _maxFrame;

    private readonly FrameDeltaType _frameDelta;
    private readonly GadgetLayerColorData _gadgetLayerColorData;
    private int _frame;
    private bool _animationFinished;

    public int Frame => _frame;
    public bool AnimationFinished => _animationFinished;

    public static AnimationLayerBehaviour CreateIncrementAnimationLayerBehaviour(
        int id,
        GadgetBehaviourName gadgetBehaviourName,
        int layer,
        int minFrame,
        int maxFrame,
        int initialFrame,
        GadgetLayerColorData gadgetLayerColorData)
    {
        return new AnimationLayerBehaviour(
            layer,
            minFrame,
            maxFrame,
            FrameDeltaType.Increment,
            initialFrame,
            gadgetLayerColorData)
        {
            Id = id,
            GadgetBehaviourName = gadgetBehaviourName,
            MaxTriggerCountPerTick = 1,
        };
    }

    public static AnimationLayerBehaviour CreateDecrementAnimationLayerBehaviour(
        int id,
        GadgetBehaviourName gadgetBehaviourName,
        int layer,
        int minFrame,
        int maxFrame,
        int initialFrame,
        GadgetLayerColorData gadgetLayerColorData)
    {
        return new AnimationLayerBehaviour(
            layer,
            minFrame,
            maxFrame,
            FrameDeltaType.Decrement,
            initialFrame,
            gadgetLayerColorData)
        {
            Id = id,
            GadgetBehaviourName = gadgetBehaviourName,
            MaxTriggerCountPerTick = 1,
        };
    }

    private AnimationLayerBehaviour(
        int layer,
        int minFrame,
        int maxFrame,
        FrameDeltaType frameDelta,
        int initialFrame,
        GadgetLayerColorData gadgetLayerColorData)
        : base(GadgetBehaviourType.GadgetAnimationRenderLayer)
    {
        _layer = layer;
        _minFrame = minFrame;
        _maxFrame = maxFrame;
        _frameDelta = frameDelta;
        _frame = initialFrame;
        _gadgetLayerColorData = gadgetLayerColorData;
    }

    protected override void PerformInternalBehaviour(int triggerData)
    {
        if (_frameDelta == FrameDeltaType.Increment)
        {
            if (_frame == _maxFrame)
            {
                _animationFinished = true;
                _frame = _minFrame;
                return;
            }
        }
        else
        {
            if (_frame == _minFrame)
            {
                _animationFinished = true;
                _frame = _maxFrame;
                return;
            }
        }

        _frame += (int)_frameDelta;
    }
    
    protected override void OnReset()
    {
        _animationFinished = false;
    }

    private enum FrameDeltaType
    {
        Decrement = -1,
        Increment = 1,
    }
}
