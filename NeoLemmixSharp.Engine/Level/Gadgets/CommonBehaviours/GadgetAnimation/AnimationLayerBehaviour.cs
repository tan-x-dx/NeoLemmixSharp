using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.GadgetAnimation;

public sealed class AnimationLayerBehaviour : GadgetBehaviour
{
    private readonly PointerWrapper _frame;
    private readonly PointerWrapper _animationFinished;

    private readonly int _layer;
    private readonly int _minFrame;
    private readonly int _maxFrame;

    private readonly FrameDeltaType _frameDelta;
    private readonly GadgetLayerColorData _gadgetLayerColorData;

    public int Frame => _frame.IntValue;
    public bool AnimationFinished => _animationFinished.BoolValue;

    public static AnimationLayerBehaviour CreateIncrementAnimationLayerBehaviour(
        nint dataHandle,
        int id,
        GadgetBehaviourName gadgetBehaviourName,
        int layer,
        int minFrame,
        int maxFrame,
        int initialFrame,
        GadgetLayerColorData gadgetLayerColorData)
    {
        return new AnimationLayerBehaviour(
            dataHandle,
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
        nint dataHandle,
        int id,
        GadgetBehaviourName gadgetBehaviourName,
        int layer,
        int minFrame,
        int maxFrame,
        int initialFrame,
        GadgetLayerColorData gadgetLayerColorData)
    {
        return new AnimationLayerBehaviour(
            dataHandle,
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
        nint dataHandle,
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
        _frame = new PointerWrapper(dataHandle);
        _frame.IntValue = initialFrame;
        _animationFinished = new PointerWrapper(dataHandle + sizeof(int));
        _gadgetLayerColorData = gadgetLayerColorData;
    }

    protected override void PerformInternalBehaviour(int triggerData)
    {
        if (_frameDelta == FrameDeltaType.Increment)
        {
            if (_frame.IntValue == _maxFrame)
            {
                _animationFinished.BoolValue = true;
                _frame.IntValue = _minFrame;
                return;
            }
        }
        else
        {
            if (_frame.IntValue == _minFrame)
            {
                _animationFinished.BoolValue = true;
                _frame.IntValue = _maxFrame;
                return;
            }
        }

        _frame.IntValue += (int)_frameDelta;
    }

    protected override void OnReset()
    {
        _animationFinished.BoolValue = false;
    }

    private enum FrameDeltaType
    {
        Decrement = -1,
        Increment = 1,
    }
}
