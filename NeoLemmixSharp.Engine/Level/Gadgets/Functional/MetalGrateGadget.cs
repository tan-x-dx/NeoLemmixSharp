using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class MetalGrateGadget : GadgetBase
{
    private int _transitionTick;
    private bool _isActive;

    public override GadgetType Type => GadgetType.MetalGrate;
    public override Orientation Orientation => DownOrientation.Instance;
    public MetalGrateState CurrentState { get; private set; }

    public MetalGrateGadgetInput Input { get; }

    public MetalGrateGadget(int id, RectangularLevelRegion gadgetBounds, MetalGrateGadgetInput input, bool startActive)
        : base(id, gadgetBounds)
    {
        Input = input;

        Input.SetMetalGrateGadget(this);

        _isActive = startActive;
    }

    public override void Tick()
    {
        switch (CurrentState)
        {
            case MetalGrateState.Off:
                {
                    if (_isActive)
                    {
                        CurrentState = MetalGrateState.Activating;
                    }

                    return;
                }
            case MetalGrateState.Activating:
                {
                    if (_transitionTick < 4)
                    {
                        _transitionTick++;
                    }
                    else
                    {
                        CurrentState = MetalGrateState.On;
                    }

                    return;
                }
            case MetalGrateState.On:
                {
                    if (!_isActive)
                    {
                        CurrentState = MetalGrateState.Deactivating;
                    }

                    return;
                }
            case MetalGrateState.Deactivating:
                {
                    if (_transitionTick > 0)
                    {
                        _transitionTick--;
                    }
                    else
                    {
                        CurrentState = MetalGrateState.Off;
                    }

                    return;
                }
        }
    }

    public override IGadgetInput? GetInputWithName(string inputName)
    {
        if (string.Equals(inputName, Input.InputName))
            return Input;

        return null;
    }

    public override bool CaresAboutLemmingInteraction => true;

    public override bool MatchesLemming(Lemming lemming)
    {
        return CurrentState == MetalGrateState.On && GadgetBounds.ContainsPoint(lemming.LevelPosition);
    }

    public override bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition) => MatchesPosition(levelPosition);

    public override void OnLemmingMatch(Lemming lemming)
    {
        if (CurrentState == MetalGrateState.Activating)
        {
            LemmingManager.RemoveLemming(lemming);
        }
    }

    public override bool MatchesPosition(LevelPosition levelPosition)
    {
        return CurrentState == MetalGrateState.On && GadgetBounds.ContainsPoint(levelPosition);
    }

    public sealed class MetalGrateGadgetInput : IGadgetInput
    {
        private MetalGrateGadget _metalGrateGadget;

        public string InputName { get; }

        public MetalGrateGadgetInput(string inputName)
        {
            InputName = inputName;
        }

        public void SetMetalGrateGadget(MetalGrateGadget metalGrateGadget)
        {
            _metalGrateGadget = metalGrateGadget;
        }

        public void ReactToSignal(bool signal)
        {
            _metalGrateGadget._isActive = signal;
        }
    }

    public enum MetalGrateState
    {
        Off,
        Activating,
        On,
        Deactivating,
    }
}