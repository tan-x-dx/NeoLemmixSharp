namespace NeoLemmixSharp.Common.Util.GameInput;

public sealed class InputAction : IInputAction
{
    private const ulong EnabledMask = (1UL << 51) - 1UL;
    private const ulong DisabledMask = 0UL;

    private const ulong ActionUnpressed = 0UL;
    public const ulong ActionPressed = 1UL;
    private const ulong ActionReleased = 2UL;
    private const ulong ActionHeld = 3UL;
    private const ulong DoubleTapUpperMask = ((1UL << 11) - 1UL) << 2;

    private ulong _stateMask = EnabledMask;
    private ulong _actionState;

    public ulong ActionState
    {
        get => _actionState;
        set => _actionState = value & _stateMask;
    }

    public void UpdateState()
    {
        _actionState = (_actionState << 1) & _stateMask;
    }

    public void SetEnabled(bool enable)
    {
        _stateMask = enable ? EnabledMask : DisabledMask;
    }

    /// <summary>
    /// Is the Action currently pressed down?
    /// </summary>
    public bool IsActionDown => (_actionState & ActionPressed) == ActionPressed;
    /// <summary>
    /// Is the Action currently released?
    /// </summary>
    public bool IsActionUp => (_actionState & ActionReleased) == ActionUnpressed;
    /// <summary>
    /// Is the Action currently pressed down, but it was previously released?
    /// </summary>
    public bool IsPressed => (_actionState & ActionHeld) == ActionPressed;
    /// <summary>
    /// Is the Action currently released, but it was previously pressed down?
    /// </summary>
    public bool IsReleased => (_actionState & ActionHeld) == ActionReleased;
    /// <summary>
    /// Is the Action currently being pressed down and it was previously pressed down?
    /// </summary>
    public bool IsHeld => (_actionState & ActionHeld) == ActionHeld;
    /// <summary>
    /// Has a double tap occurred for the Action? Double tap has about 1/4 of a second to take place.
    /// </summary>
    public bool IsDoubleTap => (_actionState & ActionPressed) != 0UL &&
                               (_actionState & ActionReleased) == 0UL &&
                               (_actionState & DoubleTapUpperMask) != 0UL;

    public bool IsEnabled => _stateMask != DisabledMask;
}