using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class MetalGrateGadget : GadgetBase
{
    private GadgetType _currentGadgetType;
    private int _transitionTick;
    private bool _isActive;

    public override GadgetType Type => _currentGadgetType;
    public override Orientation Orientation => DownOrientation.Instance;

    public MetalGrateGadgetInput Input { get; }

    public MetalGrateGadget(int id, RectangularLevelRegion gadgetBounds, MetalGrateGadgetInput input, bool startActive)
        : base(id, gadgetBounds)
    {
        Input = input;

        Input.SetMetalGrateGadget(this);

        _isActive = startActive;
        _currentGadgetType = startActive
            ? GadgetType.MetalGrateOn
            : GadgetType.MetalGrateOff;
    }

    public override void Tick()
    {
        switch (_currentGadgetType)
        {
            case GadgetType.MetalGrateOff:
                {
                    if (_isActive)
                    {
                        _currentGadgetType = GadgetType.MetalGrateActivating;
                    }

                    return;
                }
            case GadgetType.MetalGrateActivating:
                {
                    if (_transitionTick < 4)
                    {
                        _transitionTick++;
                    }
                    else
                    {
                        _currentGadgetType = GadgetType.MetalGrateOn;
                    }

                    return;
                }
            case GadgetType.MetalGrateOn:
                {
                    if (!_isActive)
                    {
                        _currentGadgetType = GadgetType.MetalGrateDeactivating;
                    }

                    return;
                }
            case GadgetType.MetalGrateDeactivating:
                {
                    if (_transitionTick > 0)
                    {
                        _transitionTick--;
                    }
                    else
                    {
                        _currentGadgetType = GadgetType.MetalGrateOff;
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
        return _currentGadgetType == GadgetType.MetalGrateOn && GadgetBounds.ContainsPoint(lemming.LevelPosition);
    }

    public override void OnLemmingMatch(Lemming lemming)
    {
    }

    public override bool MatchesPosition(LevelPosition levelPosition)
    {
        return _currentGadgetType == GadgetType.MetalGrateOn && GadgetBounds.ContainsPoint(levelPosition);
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
}