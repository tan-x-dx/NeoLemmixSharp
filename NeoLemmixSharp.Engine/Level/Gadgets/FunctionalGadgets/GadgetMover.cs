﻿using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class GadgetMover : GadgetBase, ISimpleRenderGadget
{
    private SimpleGadgetRenderer _renderer;

    private readonly int _tickDelay;
    private readonly int _dx;
    private readonly int _dy;

    private readonly IMoveableGadget[] _gadgets;

    private bool _active = true;
    private int _tickCount;

    public override SimpleGadgetRenderer Renderer => _renderer;

    public GadgetMover(
        int id,
        Orientation orientation,
        GadgetBounds gadgetBounds,
        string inputName,
        IMoveableGadget[] gadgets,
        int tickDelay,
        int dx,
        int dy)
        : base(id, orientation, gadgetBounds, 1)
    {
        _tickDelay = tickDelay;
        _gadgets = gadgets;
        _dx = dx;
        _dy = dy;

        RegisterInput(new GadgetMoverInput(inputName, this));
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

        for (var i = 0; i < _gadgets.Length; i++)
        {
            _gadgets[i].Move(_dx, _dy);
        }
    }

    private sealed class GadgetMoverInput : GadgetInput
    {
        private readonly GadgetMover _mover;

        public GadgetMoverInput(string inputName, GadgetMover mover)
            : base(inputName)
        {
            _mover = mover;
        }

        public override void OnRegistered()
        {
            _mover._active = false;
        }

        public override void ReactToSignal(bool signal)
        {
            _mover._active = signal;
        }
    }

    SimpleGadgetRenderer ISimpleRenderGadget.Renderer
    {
        get => _renderer;
        set => _renderer = value;
    }
}
