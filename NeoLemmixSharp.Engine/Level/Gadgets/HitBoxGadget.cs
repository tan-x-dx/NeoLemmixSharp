using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

#pragma warning disable CS0660, CS0661, CA1067
public sealed class HitBoxGadget : GadgetBase,
    IIdEquatable<HitBoxGadget>,
    IRectangularBounds,
    IAnimationControlledGadget,
    IMoveableGadget,
    IResizeableGadget
#pragma warning restore CS0660, CS0661, CA1067
{
    private readonly GadgetLayerRenderer _renderer;
    private readonly LemmingTracker _lemmingTracker;
    private readonly GadgetState[] _states;

    private GadgetState _currentState;
    private GadgetState _previousState;

    private int _currentStateIndex;
    private int _nextStateIndex;

    public GadgetState CurrentState => _currentState;
    public GadgetStateAnimationController AnimationController { get; private set; }

    public override GadgetLayerRenderer Renderer => _renderer;

    // The below properties refer to the position of the hitbox, not the gadget itself    
    public LevelPosition TopLeftPixel => _currentGadgetBounds.TopLeftPosition + _currentState.HitBoxRegion.Offset;
    public LevelPosition BottomRightPixel => _currentGadgetBounds.BottomRightPosition + _currentState.HitBoxRegion.Offset + _currentState.HitBoxRegion.BoundingBoxDimensions;
    public LevelPosition PreviousTopLeftPixel => _previousGadgetBounds.TopLeftPosition + _previousState.HitBoxRegion.Offset;
    public LevelPosition PreviousBottomRightPixel => _previousGadgetBounds.BottomRightPosition + _previousState.HitBoxRegion.Offset + _previousState.HitBoxRegion.BoundingBoxDimensions;
    public ResizeType ResizeType { get; }

    public HitBoxGadget(
        int id,
        Orientation orientation,
        GadgetBounds initialGadgetBounds,
        ResizeType resizeType,
        LemmingTracker lemmingTracker,
        GadgetState[] states,
        int initialStateIndex)
        : base(id, orientation, initialGadgetBounds)
    {
        _lemmingTracker = lemmingTracker;
        _states = states;

        _currentStateIndex = initialStateIndex;
        _currentState = states[initialStateIndex];
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

        _previousGadgetBounds.SetFrom(_currentGadgetBounds);
        _previousState = _currentState;

        _currentState = _states[_currentStateIndex];
        AnimationController = CurrentState.AnimationController;

        _previousState.OnTransitionFrom();
        CurrentState.OnTransitionTo();

        // Changing states may change hitbox positions 
        // Force a position update to accommodate this
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public bool ContainsPoint(LevelPosition levelPosition)
    {
        var p = levelPosition - TopLeftPixel;

        return _currentState.HitBoxRegion.ContainsPoint(p);
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
        _previousGadgetBounds.SetFrom(_currentGadgetBounds);
        _previousState = _currentState;

        _currentGadgetBounds.X = LevelScreen.HorizontalBoundaryBehaviour.Normalise(_currentGadgetBounds.X + dx);
        _currentGadgetBounds.Y = LevelScreen.VerticalBoundaryBehaviour.Normalise(_currentGadgetBounds.Y + dy);
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void SetPosition(int x, int y)
    {
        _previousGadgetBounds.SetFrom(_currentGadgetBounds);
        _previousState = _currentState;

        _currentGadgetBounds.X = LevelScreen.HorizontalBoundaryBehaviour.Normalise(x);
        _currentGadgetBounds.Y = LevelScreen.VerticalBoundaryBehaviour.Normalise(y);
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void Resize(int dw, int dh)
    {
        if (ResizeType == ResizeType.None)
            return;

        _previousGadgetBounds.SetFrom(_currentGadgetBounds);
        _previousState = _currentState;

        if (ResizeType.HasFlag(ResizeType.ResizeHorizontal))
        {
            _currentGadgetBounds.Width += dw;
        }
        if (ResizeType.HasFlag(ResizeType.ResizeVertical))
        {
            _currentGadgetBounds.Height += dh;
        }
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void SetSize(int w, int h)
    {
        if (ResizeType == ResizeType.None)
            return;

        _previousGadgetBounds.SetFrom(_currentGadgetBounds);
        _previousState = _currentState;

        if (ResizeType.HasFlag(ResizeType.ResizeHorizontal))
        {
            _currentGadgetBounds.Width = w;
        }
        if (ResizeType.HasFlag(ResizeType.ResizeVertical))
        {
            _currentGadgetBounds.Height = h;
        }
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public bool Equals(HitBoxGadget? other) => Id == (other?.Id ?? -1);

    public static bool operator ==(HitBoxGadget left, HitBoxGadget right) => left.Id == right.Id;
    public static bool operator !=(HitBoxGadget left, HitBoxGadget right) => left.Id != right.Id;
}
