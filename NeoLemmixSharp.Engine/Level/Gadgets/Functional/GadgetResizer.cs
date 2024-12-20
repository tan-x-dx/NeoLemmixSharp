﻿using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class GadgetResizer : GadgetBase, IReactiveGadget
{
    private readonly int _tickDelay;
    private readonly int _dw;
    private readonly int _dh;

    private readonly IResizeableGadget[] _gadgets;

    private bool _active = true;
    private int _tickCount;

    public override GadgetBehaviour GadgetBehaviour => FunctionalGadgetBehaviour.Instance;
    public override Orientation Orientation => DownOrientation.Instance;

    public GadgetResizerInput Input { get; }

    public GadgetResizer(
        int id,
        RectangularHitBoxRegion gadgetBounds,
        IControlledAnimationGadgetRenderer? renderer,
        IResizeableGadget[] gadgets,
        int tickDelay,
        int dw,
        int dh)
        : base(id, gadgetBounds, renderer)
    {
        _tickDelay = tickDelay;
        _gadgets = gadgets;
        _dw = dw;
        _dh = dh;

        Input = new GadgetResizerInput("Input", this);
    }

    public override void Tick()
    {
        if (!_active)
            return;

        if (_tickCount < _tickDelay)
        {
            _tickCount++;
            return;
        }

        _tickCount = 0;

        foreach (var gadget in _gadgets.AsSpan())
        {
            gadget.SetSize(_dw, _dh);
        }
    }

    public IGadgetInput? GetInputWithName(string inputName)
    {
        if (string.Equals(inputName, Input.InputName))
            return Input;
        return null;
    }

    public sealed class GadgetResizerInput : IGadgetInput
    {
        private readonly GadgetResizer _resizer;
        public string InputName { get; }

        public GadgetResizerInput(string inputName, GadgetResizer resizer)
        {
            InputName = inputName;
            _resizer = resizer;
        }

        public void OnRegistered()
        {
            _resizer._active = false;
        }

        public void ReactToSignal(bool signal)
        {
            _resizer._active = signal;
        }
    }
}