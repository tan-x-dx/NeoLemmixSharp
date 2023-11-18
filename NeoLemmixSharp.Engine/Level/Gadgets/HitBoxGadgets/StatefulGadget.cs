using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetBehaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using static NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxHelpers;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class StatefulGadget : HitBoxGadget, IMoveableGadget
{
    private readonly HitBox _hitBox;
    private readonly GadgetState[] _states;
    private readonly ItemTracker<Lemming> _itemTracker;

    private int _currentStateIndex;
    private int _nextStateIndex;

    public override InteractiveGadgetType SubType { get; }
    public override Orientation Orientation { get; }

    public StatefulGadget(
        int id,
        InteractiveGadgetType type,
        Orientation orientation,
        RectangularLevelRegion gadgetBounds,
        HitBox hitBox,
        GadgetState[] states,
        ItemTracker<Lemming> itemTracker)
        : base(id, gadgetBounds)
    {
        SubType = type;
        Orientation = orientation;
        _hitBox = hitBox;
        _states = states;
        _itemTracker = itemTracker;

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
        var itemStatus = _itemTracker.TrackItem(lemming);

        var state = _states[_currentStateIndex];
        ReadOnlySpan<IGadgetBehaviour> actions;

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
            actions = ReadOnlySpan<IGadgetBehaviour>.Empty;
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
        _itemTracker.Tick();

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