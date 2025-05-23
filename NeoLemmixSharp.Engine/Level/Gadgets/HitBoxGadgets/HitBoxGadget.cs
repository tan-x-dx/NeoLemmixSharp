﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

#pragma warning disable CS0660, CS0661, CA1067
public sealed class HitBoxGadget : GadgetBase,
    IIdEquatable<HitBoxGadget>,
    IRectangularBounds,
    IMoveableGadget
#pragma warning restore CS0660, CS0661, CA1067
{
    private readonly LemmingTracker _lemmingTracker;
    private readonly GadgetState[] _states;

    private GadgetState _currentState;
    private GadgetState _previousState;

    private int _currentStateIndex;
    private int _nextStateIndex;

    public GadgetState CurrentState => _currentState;

    // The below properties refer to the positions of the hitboxes, not the gadget itself
    public RectangularRegion CurrentBounds => _currentState.GetMininmumBoundingBoxForAllHitBoxes(CurrentGadgetBounds.Position);

    public ResizeType ResizeType { get; }

    public HitBoxGadget(
        ResizeType resizeType,
        LemmingTracker lemmingTracker,
        GadgetState[] states,
        int initialStateIndex)
        : base(0)
    {
        _lemmingTracker = lemmingTracker;
        _states = states;

        _currentStateIndex = initialStateIndex;
        _currentState = _states[initialStateIndex];
        CurrentAnimationController = _currentState.AnimationController;
        _previousState = _currentState;

        ResizeType = resizeType;
    }

    public void SetNextState(int stateIndex)
    {
        _nextStateIndex = stateIndex;
    }

    public override void Tick()
    {
        _lemmingTracker.Tick();

        if (_currentStateIndex == _nextStateIndex)
        {
            CurrentState.Tick(this);
        }
        else
        {
            ChangeStates();
        }
    }

    private void ChangeStates()
    {
        _currentStateIndex = _nextStateIndex;

        _previousState = _currentState;

        _currentState = _states[_currentStateIndex];

        _previousState.OnTransitionFrom();
        _currentState.OnTransitionTo();
        CurrentAnimationController = _currentState.AnimationController;

        // Changing states may change hitbox positions 
        // Force a position update to accommodate this
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public bool ContainsPoint(Orientation orientation, Point levelPosition)
    {
        return _currentState
            .HitBoxFor(orientation)
            .ContainsPoint(levelPosition - CurrentGadgetBounds.Position);
    }

    public bool ContainsEitherPoint(Orientation orientation, Point p1, Point p2)
    {
        var offset = CurrentGadgetBounds.Position;
        var hitBox = _currentState.HitBoxFor(orientation);
        return hitBox.ContainsEitherPoint(p1 - offset, p2 - offset);
    }

    public void OnLemmingHit(
        LemmingHitBoxFilter activeFilter,
        Lemming lemming)
    {
        var actionsToPerform = GetActionsToPerformOnLemming(activeFilter, lemming);

        for (var i = 0; i < actionsToPerform.Length; i++)
        {
            actionsToPerform[i].PerformAction(lemming);
        }
    }

    private ReadOnlySpan<IGadgetAction> GetActionsToPerformOnLemming(
        LemmingHitBoxFilter activeFilter,
        Lemming lemming)
    {
        var trackingStatus = _lemmingTracker.TrackItem(lemming);

        return trackingStatus switch
        {
            TrackingStatus.Absent => ReadOnlySpan<IGadgetAction>.Empty,
            TrackingStatus.JustAdded => activeFilter.OnLemmingEnterActions,
            TrackingStatus.JustRemoved => activeFilter.OnLemmingExitActions,
            TrackingStatus.StillPresent => activeFilter.OnLemmingPresentActions,

            _ => ReadOnlySpan<IGadgetAction>.Empty
        };
    }

    public void Move(int dx, int dy)
    {
        _previousState = _currentState;

        var delta = new Point(dx, dy);
        CurrentGadgetBounds.Position = LevelScreen.NormalisePosition(CurrentGadgetBounds.Position + delta);
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void SetPosition(int x, int y)
    {
        _previousState = _currentState;

        var newPosition = new Point(x, y);
        CurrentGadgetBounds.Position = LevelScreen.NormalisePosition(newPosition);
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void Resize(int dw, int dh)
    {
        if (ResizeType == ResizeType.None)
            return;

        _previousState = _currentState;

        if (ResizeType.CanResizeHorizontally())
            CurrentGadgetBounds.Width += dw;

        if (ResizeType.CanResizeVertically())
            CurrentGadgetBounds.Height += dh;

        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void SetSize(int w, int h)
    {
        if (ResizeType == ResizeType.None)
            return;

        _previousState = _currentState;

        if (ResizeType.CanResizeHorizontally())
            CurrentGadgetBounds.Width = w;

        if (ResizeType.CanResizeVertically())
            CurrentGadgetBounds.Height = h;

        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public bool Equals(HitBoxGadget? other) => Id == (other?.Id ?? -1);

    public static bool operator ==(HitBoxGadget left, HitBoxGadget right) => left.Id == right.Id;
    public static bool operator !=(HitBoxGadget left, HitBoxGadget right) => left.Id != right.Id;
}
