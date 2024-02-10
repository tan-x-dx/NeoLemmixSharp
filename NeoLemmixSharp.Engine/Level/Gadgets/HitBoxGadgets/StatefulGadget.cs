using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxHelpers;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class StatefulGadget : HitBoxGadget, IMoveableGadget
{
    private readonly HitBox _hitBox;
    private readonly GadgetState[] _states;

    private int _currentStateIndex;
    private int _nextStateIndex;

    public override GadgetSubType GadgetSubType { get; }
    public override Orientation Orientation { get; }

    public StatefulGadget(
        int id,
        GadgetSubType interactionType,
        Orientation orientation,
        RectangularLevelRegion gadgetBounds,
        HitBox hitBox,
        GadgetState[] states,
        ItemTracker<Lemming> lemmingTracker)
        : base(id, gadgetBounds, lemmingTracker)
    {
        GadgetSubType = interactionType;
        Orientation = orientation;
        _hitBox = hitBox;
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
        return _hitBox.MatchesLemming(lemming) &&
               MatchesPosition(levelPosition);
    }

    public override bool MatchesPosition(LevelPosition levelPosition)
    {
        levelPosition = LevelRegionHelpers.GetRelativePosition(TopLeftPixel, levelPosition);

        return _hitBox.MatchesPosition(levelPosition);
    }

    public override void OnLemmingMatch(Lemming lemming)
    {
        var itemStatus = LemmingTracker.TrackItem(lemming);

        var state = _states[_currentStateIndex];
        ReadOnlySpan<IGadgetAction> actions;

        if (IsItemPresent(itemStatus))
        {
            actions = state.OnLemmingPresentActions;
        }
        else if (IsItemAdded(itemStatus))
        {
            actions = state.OnLemmingEnterActions;
        }
        else if (IsItemRemoved(itemStatus))
        {
            actions = state.OnLemmingExitActions;
        }
        else
        {
            actions = ReadOnlySpan<IGadgetAction>.Empty;
        }

        foreach (var action in actions)
        {
            action.PerformAction(lemming);
        }
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

        var state = _states[_currentStateIndex];
        state.Tick();
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