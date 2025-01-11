using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

#pragma warning disable CS0660, CS0661, CA1067
public sealed class HitBoxGadget : GadgetBase, IIdEquatable<HitBoxGadget>, IRectangularBounds, IAnimationControlledGadget
#pragma warning restore CS0660, CS0661, CA1067
{
    private readonly GadgetLayerRenderer _renderer;
    private readonly LemmingTracker _lemmingTracker;
    private readonly GadgetState[] _states;

    private int _currentX;
    private int _currentY;
    private int _currentWidth;
    private int _currentHeight;

    private int _previousX;
    private int _previousY;
    private int _previousWidth;
    private int _previousHeight;

    private int _currentStateIndex;
    private int _nextStateIndex;

    public GadgetState CurrentState { get; private set; }
    public GadgetStateAnimationController AnimationController { get; private set; }

    public override GadgetLayerRenderer Renderer => _renderer;

    public LevelPosition TopLeftPixel => new(_currentX, _currentY);
    public LevelPosition BottomRightPixel => new(_currentX + _currentWidth, _currentY + _currentHeight);
    public LevelPosition PreviousTopLeftPixel => new(_previousX, _previousY);
    public LevelPosition PreviousBottomRightPixel => new(_previousX + _previousWidth, _previousY + _previousHeight);
    public bool IsResizable { get; }

    public HitBoxGadget(
        int id,
        LevelRegion gadgetBounds,
        Orientation orientation,
        LemmingTracker lemmingTracker,
        GadgetState[] states)
        : base(id, orientation)
    {
        _lemmingTracker = lemmingTracker;
        _states = states;

        var p0 = gadgetBounds.GetTopLeftPosition();
        var p1 = gadgetBounds.GetBottomRightPosition();
        _currentX = p0.X;
        _currentY = p0.Y;
        _currentWidth = 1 + p1.X - p0.X;
        _currentHeight = 1 + p1.Y - p0.Y;

        _previousX = p0.X;
        _previousY = p0.Y;
        _previousWidth = _currentWidth;
        _previousHeight = _currentHeight;

        IsResizable = ValidateAllHitBoxesAreResizable();
    }

    private bool ValidateAllHitBoxesAreResizable()
    {
        const int resizableType = 1;
        const int nonResizableType = 2;

        var type = 0;

        for (var i = 0; i < _states.Length; i++)
        {
            var hitBoxRegion = _states[i].HitBoxRegion;

            if (hitBoxRegion is IResizableHitBoxRegion)
            {
                if (type == nonResizableType)
                    throw new InvalidOperationException("Inconsistent resizability for gadget!");
                type = resizableType;
            }
            else
            {
                if (type == resizableType)
                    throw new InvalidOperationException("Inconsistent resizability for gadget!");
                type = nonResizableType;
            }
        }

        if (type == 0)
            throw new InvalidOperationException("Could not determine resizability for gadget!");

        return type == resizableType;
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

        CurrentState.Tick(this);
    }

    private void ChangeStates()
    {
        var previousState = _states[_currentStateIndex];

        _currentStateIndex = _nextStateIndex;

        CurrentState = _states[_currentStateIndex];
        AnimationController = CurrentState.AnimationController;

        previousState.OnTransitionFrom();
        CurrentState.OnTransitionTo();
    }

    public bool ContainsPoint(LevelPosition levelPosition)
    {
        var currentState = CurrentState;
        var p = levelPosition - TopLeftPixel - currentState.HitBoxOffset;

        return currentState.HitBoxRegion.ContainsPoint(p);
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
        _previousX = _currentX;
        _previousY = _currentY;
        _previousWidth = _currentWidth;
        _previousHeight = _currentHeight;

        _currentX = LevelScreen.HorizontalBoundaryBehaviour.Normalise(_currentX + dx);
        _currentY = LevelScreen.VerticalBoundaryBehaviour.Normalise(_currentY + dy);
    }

    public void SetPosition(int x, int y)
    {
        _previousX = _currentX;
        _previousY = _currentY;
        _previousWidth = _currentWidth;
        _previousHeight = _currentHeight;

        _currentX = LevelScreen.HorizontalBoundaryBehaviour.Normalise(x);
        _currentY = LevelScreen.VerticalBoundaryBehaviour.Normalise(y);
    }

    public void Resize(int dw, int dh)
    {
        for (var i = 0; i < _states.Length; i++)
        {
            var rectangularHitBox = (IResizableHitBoxRegion)_states[i].HitBoxRegion;
            rectangularHitBox.Resize(dw, dh);
        }

        _previousX = _currentX;
        _previousY = _currentY;
        _previousWidth = _currentWidth;
        _previousHeight = _currentHeight;

        _currentWidth = Math.Max(0, _currentWidth + dw);
        _currentHeight = Math.Max(0, _currentHeight + dh);
    }

    public void SetSize(int w, int h)
    {
        for (var i = 0; i < _states.Length; i++)
        {
            var rectangularHitBox = (IResizableHitBoxRegion)_states[i].HitBoxRegion;
            rectangularHitBox.SetSize(w, h);
        }

        _previousX = _currentX;
        _previousY = _currentY;
        _previousWidth = _currentWidth;
        _previousHeight = _currentHeight;

        _currentWidth = Math.Max(0, w);
        _currentHeight = Math.Max(0, h);
    }

    public bool Equals(HitBoxGadget? other) => Id == (other?.Id ?? -1);

    public static bool operator ==(HitBoxGadget left, HitBoxGadget right) => left.Id == right.Id;
    public static bool operator !=(HitBoxGadget left, HitBoxGadget right) => left.Id != right.Id;
}
