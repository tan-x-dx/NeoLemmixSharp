using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.States;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class GadgetMover : GadgetBase
{
    private readonly int _tickDelay;
    private readonly int _dx;
    private readonly int _dy;

    private readonly StatefulGadget[] _gadgets;

    private int _tickCount;

    public override GadgetType Type => GadgetType.Mover;
    public override Orientation Orientation => DownOrientation.Instance;

    public GadgetMover(
        int id,
        RectangularLevelRegion gadgetBounds,
        StatefulGadget[] gadgets,
        int tickDelay,
        int dx,
        int dy)
        : base(id, gadgetBounds)
    {
        _tickDelay = tickDelay;
        _gadgets = gadgets;
        _dx = dx;
        _dy = dy;
    }

    public override void Tick()
    {
        if (_tickCount < _tickDelay)
        {
            _tickCount++;
            return;
        }

        _tickCount = 0;

        for (var i = 0; i < _gadgets.Length; i++)
        {
            var gadget = _gadgets[i];
            var movementBehaviour = gadget.CurrentState.MovementBehaviour;
            movementBehaviour.Move(_dx, _dy);
        }
    }

    public override IGadgetInput? GetInputWithName(string inputName)
    {
        throw new NotImplementedException();
    }

    public override bool CaresAboutLemmingInteraction => false;
    public override bool MatchesLemming(Lemming lemming) => false;
    public override void OnLemmingMatch(Lemming lemming)
    {
    }

    public override bool MatchesPosition(LevelPosition levelPosition) => false;
}