using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using static NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxHelpers;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;

public sealed class StatefulGadget : HitBoxGadget, IMoveableGadget
{
    private readonly GadgetState[] _states;

    private int _currentStateIndex;
    private int _nextStateIndex;

    public override GadgetBehaviour GadgetBehaviour => _states[_currentStateIndex].GadgetBehaviour;
    public override Orientation Orientation { get; }

    public StatefulGadget(
        int id,
        Orientation orientation,
        RectangularLevelRegion gadgetBounds,
        IGadgetRenderer? renderer,
        GadgetState[] states,
        ItemTracker<Lemming> lemmingTracker)
        : base(id, gadgetBounds, renderer, lemmingTracker)
    {
        Orientation = orientation;
        _states = states;

        foreach (var gadgetState in _states)
        {
            gadgetState.SetGadget(this);
        }
    }

    public void SetNextState(int stateIndex)
    {
        _nextStateIndex = stateIndex;
    }

    public override bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition)
    {
        var hitBox = _states[_currentStateIndex].HitBox;
        return hitBox.MatchesLemming(lemming) &&
               hitBox.MatchesPosition(levelPosition);
    }

    public override bool MatchesPosition(LevelPosition levelPosition)
    {
        levelPosition = LevelRegionHelpers.GetRelativePosition(TopLeftPixel, levelPosition);

        return _states[_currentStateIndex].HitBox.MatchesPosition(levelPosition);
    }

    public override void OnLemmingMatch(Lemming lemming)
    {
        var actionsToPerform = GetActionsToPerformOnLemming(lemming);

        foreach (var action in actionsToPerform)
        {
            action.PerformAction(lemming);
        }
    }

    private ReadOnlySpan<IGadgetAction> GetActionsToPerformOnLemming(Lemming lemming)
    {
        var gadgetState = _states[_currentStateIndex];
        var itemStatus = LemmingTracker.TrackItem(lemming);

        if (IsItemPresent(itemStatus))
            return gadgetState.OnLemmingPresentActions;

        if (IsItemAdded(itemStatus))
            return gadgetState.OnLemmingEnterActions;

        if (IsItemRemoved(itemStatus))
            return gadgetState.OnLemmingExitActions;

        return ReadOnlySpan<IGadgetAction>.Empty;
    }

    public void Move(int dx, int dy)
    {
        var newPosition = TopLeftPixel + new LevelPosition(dx, dy);

        UpdatePosition(newPosition);
    }

    public void SetPosition(int x, int y)
    {
        var newPosition = new LevelPosition(x, y);

        UpdatePosition(newPosition);
    }

    public override void Tick()
    {
        LemmingTracker.Tick();

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
}