using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

#pragma warning disable CS0660, CS0661, CA1067
public sealed class HitBoxGadget : GadgetBase, IIdEquatable<HitBoxGadget>, IRectangularBounds
#pragma warning restore CS0660, CS0661, CA1067
{
    private readonly IGadgetRenderer _renderer;
    private readonly LemmingTracker _lemmingTracker;
    private readonly GadgetState[] _states;

    private int _currentStateIndex;
    private int _nextStateIndex;

    public HitBox HitBox => _states[_currentStateIndex].HitBox;
    public GadgetStateAnimationController AnimationController => _states[_currentStateIndex].AnimationController;

    public override IGadgetRenderer Renderer => _renderer;

    public LevelPosition TopLeftPixel => HitBox.TopLeftPixel;
    public LevelPosition BottomRightPixel => HitBox.BottomRightPixel;
    public LevelPosition PreviousTopLeftPixel => HitBox.PreviousTopLeftPixel;
    public LevelPosition PreviousBottomRightPixel => HitBox.PreviousBottomRightPixel;

    public HitBoxGadget(
        int id,
        LemmingTracker lemmingTracker,
        GadgetState[] states)
        : base(id)
    {
        _lemmingTracker = lemmingTracker;
        _states = states;
    }

    public bool ValidateAllHitBoxesAreResizable()
    {
        for (var i = 0; i < _states.Length; i++)
        {
            var hitBoxRegion = _states[i].HitBox.HitBoxRegion;

            if (hitBoxRegion is not RectangularHitBoxRegion)
                return false;
        }

        return true;
    }

    public void SetNextState(int stateIndex)
    {
        _nextStateIndex = stateIndex;
    }

    public override void Tick()
    {
        _lemmingTracker.Tick();

        if (_currentStateIndex != _nextStateIndex)
        {
            ChangeStates();
            return;
        }

        _states[_currentStateIndex].Tick();
    }

    private void ChangeStates()
    {
        var previousState = _states[_currentStateIndex];

        _currentStateIndex = _nextStateIndex;

        var currentState = _states[_currentStateIndex];

        previousState.OnTransitionFrom();
        currentState.OnTransitionTo();
    }

    public void OnLemmingHit(Lemming lemming)
    {
        var actionsToPerform = GetActionsToPerformOnLemming(lemming);

        for (var i = 0; i < actionsToPerform.Length; i++)
        {
            actionsToPerform[i].PerformAction(lemming);
        }
    }

    private ReadOnlySpan<IGadgetAction> GetActionsToPerformOnLemming(Lemming lemming)
    {
        var gadgetState = _states[_currentStateIndex];
        var trackingStatus = _lemmingTracker.TrackItem(lemming);

        return trackingStatus switch
        {
            TrackingStatus.Absent => ReadOnlySpan<IGadgetAction>.Empty,
            TrackingStatus.JustAdded => gadgetState.OnLemmingEnterActions,
            TrackingStatus.JustRemoved => gadgetState.OnLemmingExitActions,
            TrackingStatus.StillPresent => gadgetState.OnLemmingPresentActions,

            _ => ReadOnlySpan<IGadgetAction>.Empty
        };
    }

    public bool Equals(HitBoxGadget? other) => Id == (other?.Id ?? -1);

    public static bool operator ==(HitBoxGadget left, HitBoxGadget right) => left.Id == right.Id;
    public static bool operator !=(HitBoxGadget left, HitBoxGadget right) => left.Id != right.Id;
}
