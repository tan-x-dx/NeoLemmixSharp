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

    /// <summary>
    /// The current top left pixel of the gadget
    /// </summary>
    private LevelPosition _currentPosition;
    private LevelSize _currentSize;
    private GadgetState _currentState;

    /// <summary>
    /// The previous top left pixel of the gadget
    /// </summary>
    private LevelPosition _previousPosition;
    private LevelSize _previousSize;
    private GadgetState _previousState;

    private int _currentStateIndex;
    private int _nextStateIndex;

    public GadgetState CurrentState => _currentState;
    public GadgetStateAnimationController AnimationController { get; private set; }

    public override GadgetLayerRenderer Renderer => _renderer;

    /*
     * The below properties refer to the position of the hitbox, not the gadget itself
     * This is needed in the gadget SpacialHashGrid
     */
    public LevelPosition TopLeftPixel => _currentPosition + _currentState.HitBoxOffset;
    public LevelPosition BottomRightPixel => _currentPosition + _currentState.HitBoxOffset + _currentState.HitBoxRegion.BoundingBoxDimensions;
    public LevelPosition PreviousTopLeftPixel => _previousPosition + _previousState.HitBoxOffset;
    public LevelPosition PreviousBottomRightPixel => _previousPosition + _previousState.HitBoxOffset + _previousState.HitBoxRegion.BoundingBoxDimensions;
    public bool IsResizable { get; }

    public HitBoxGadget(
        int id,
        LevelRegion gadgetBounds,
        Orientation orientation,
        LemmingTracker lemmingTracker,
        GadgetState[] states,
        int initialStateIndex)
        : base(id, orientation)
    {
        _lemmingTracker = lemmingTracker;
        _states = states;

        _currentStateIndex = initialStateIndex;
        _currentState = states[initialStateIndex];
        _previousState = _currentState;

        _currentPosition = gadgetBounds.P1;
        _currentSize = gadgetBounds.GetSize();

        _previousPosition = _currentPosition;
        _previousSize = _currentSize;

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

        _previousState = _currentState;
        _currentState = _states[_currentStateIndex];
        AnimationController = CurrentState.AnimationController;

        previousState.OnTransitionFrom();
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
        _previousPosition = _currentPosition;
        _previousSize = _currentSize;

        _currentPosition = LevelScreen.NormalisePosition(_currentPosition + new LevelPosition(dx, dy));
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void SetPosition(int x, int y)
    {
        _previousPosition = _currentPosition;
        _previousSize = _currentSize;

        _currentPosition = LevelScreen.NormalisePosition(new LevelPosition(x, y));
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void Resize(int dw, int dh)
    {
        for (var i = 0; i < _states.Length; i++)
        {
            var rectangularHitBox = (IResizableHitBoxRegion)_states[i].HitBoxRegion;
            rectangularHitBox.Resize(dw, dh);
        }

        _previousPosition = _currentPosition;
        _previousSize = _currentSize;

        _currentSize = new LevelSize(_currentSize.W + dw, _currentSize.H + dh);
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public void SetSize(int w, int h)
    {
        for (var i = 0; i < _states.Length; i++)
        {
            var rectangularHitBox = (IResizableHitBoxRegion)_states[i].HitBoxRegion;
            rectangularHitBox.SetSize(w, h);
        }

        _previousPosition = _currentPosition;
        _previousSize = _currentSize;

        _currentSize = new LevelSize(w, h);
        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public bool Equals(HitBoxGadget? other) => Id == (other?.Id ?? -1);

    public static bool operator ==(HitBoxGadget left, HitBoxGadget right) => left.Id == right.Id;
    public static bool operator !=(HitBoxGadget left, HitBoxGadget right) => left.Id != right.Id;
}
