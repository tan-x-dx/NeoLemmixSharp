using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class MetalGrateGadget : HitBoxGadget, IReactiveGadget, IResizeableGadget
{
    private int _transitionTick;
    private bool _isActive;

    public override InteractiveGadgetType SubType => MetalGrateGadgetType.Instance;
    public override Orientation Orientation => DownOrientation.Instance;
    public MetalGrateState CurrentState { get; private set; }

    public MetalGrateGadgetInput Input { get; }

    public MetalGrateGadget(
        int id, 
        RectangularLevelRegion gadgetBounds,
        ItemTracker<Lemming> lemmingTracker,
        MetalGrateGadgetInput input,
        bool startActive)
        : base(id, gadgetBounds, lemmingTracker)
    {
        Input = input;

        Input.SetMetalGrateGadget(this);

        _isActive = startActive;
    }

    public override void Tick()
    {
        if (CurrentState == MetalGrateState.Off)
        {
            if (_isActive)
            {
                CurrentState = MetalGrateState.Activating;
            }

            return;
        }

        if (CurrentState == MetalGrateState.Activating)
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

        if (CurrentState == MetalGrateState.On)
        {
            if (!_isActive)
            {
                CurrentState = MetalGrateState.Deactivating;
            }

            return;
        }

        if (_transitionTick > 0)
        {
            _transitionTick--;
        }
        else
        {
            CurrentState = MetalGrateState.Off;
        }
    }

    void IResizeableGadget.Resize(int dw, int dh)
    {
        // Do nothing - this gadget type can have any size
        // but cannot be resized during the play through
    }

    void IResizeableGadget.SetSize(int w, int h)
    {
        // Do nothing - this gadget type can have any size
        // but cannot be resized during the play through
    }

    public IGadgetInput? GetInputWithName(string inputName)
    {
        if (string.Equals(inputName, Input.InputName))
            return Input;

        return null;
    }

    public override bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition) => MatchesPosition(levelPosition);

    public override bool MatchesPosition(LevelPosition levelPosition)
    {
        levelPosition = LevelRegionHelpers.GetRelativePosition(TopLeftPixel, levelPosition);

        return CurrentState == MetalGrateState.On && GadgetBounds.ContainsPoint(levelPosition);
    }

    public override void OnLemmingMatch(Lemming lemming)
    {
        if (CurrentState == MetalGrateState.Activating)
        {
            LevelScreen.LemmingManager.RemoveLemming(lemming, LemmingRemovalReason.DeathMetalGrate);
        }
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

        public void OnRegistered()
        {
            _metalGrateGadget._isActive = false;
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