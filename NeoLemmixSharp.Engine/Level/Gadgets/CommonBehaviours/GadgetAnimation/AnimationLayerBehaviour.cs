using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.GadgetAnimation;

public sealed class AnimationLayerBehaviour : GadgetBehaviour
{
    private readonly PointerWrapper<int> _frame;
    private readonly PointerWrapper<bool> _animationFinished;

    private readonly int _layer;
    private readonly int _minFrame;
    private readonly int _maxFrame;

    private readonly FrameDeltaType _frameDelta;
    private readonly GadgetLayerColorData _gadgetLayerColorData;

    private ref int FrameRef => ref _frame.Value;
    private ref bool AnimationFinishedRef => ref _animationFinished.Value;

    public int Frame => _frame.Value;
    public bool AnimationFinished => _animationFinished.Value;

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
        _frame = new PointerWrapper<int>(dataHandle);
        _animationFinished = new PointerWrapper<bool>(dataHandle + sizeof(int));
        _gadgetLayerColorData = gadgetLayerColorData;
    }

    protected override void PerformInternalBehaviour(int triggerData)
    {
        if (_frameDelta == FrameDeltaType.Increment)
        {
            if (FrameRef == _maxFrame)
            {
                AnimationFinishedRef = true;
                FrameRef = _minFrame;
                return;
            }
        }
        else
        {
            if (FrameRef == _minFrame)
            {
                AnimationFinishedRef = true;
                FrameRef = _maxFrame;
                return;
            }
        }

        FrameRef += (int)_frameDelta;
    }

    protected override void OnReset()
    {
        AnimationFinishedRef = false;
    }

    private enum FrameDeltaType
    {
        Decrement = -1,
        Increment = 1,
    }
}
