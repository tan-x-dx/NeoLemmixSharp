using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional.BinaryLogic;

public sealed class NotGateGadget : GadgetBase, ILogicGateGadget
{
    private bool _shouldTick;

    public override GadgetType Type => GadgetType.NotGate;
    public override Orientation Orientation => DownOrientation.Instance;

    public LogicGateGadgetInput Input { get; }
    public GadgetOutput Output { get; } = new();

    public NotGateGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        LogicGateGadgetInput input)
        : base(id, gadgetBounds)
    {
        Input = input;

        Input.SetLogicGate(this);
    }

    public override void Tick()
    {
        if (_shouldTick)
            return;

        _shouldTick = false;
        Output.SetSignal(!Input.Signal);
    }

    public override IGadgetInput? GetInputWithName(string inputName)
    {
        if (string.Equals(inputName, Input.InputName))
            return Input;

        return null;
    }

    public override bool CaresAboutLemmingInteraction => false;
    public override bool MatchesLemming(Lemming lemming) => false;
    public override bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition) => false;
    public override void OnLemmingMatch(Lemming lemming)
    {
    }

    public override bool MatchesPosition(LevelPosition levelPosition) => false;

    public void EvaluateInputs()
    {
        _shouldTick = true;
    }
}