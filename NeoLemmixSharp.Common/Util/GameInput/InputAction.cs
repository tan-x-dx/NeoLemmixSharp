namespace NeoLemmixSharp.Common.Util.GameInput;

public sealed class InputAction : IInputAction
{
    private const ulong EnabledMask = (1UL << EngineConstants.FramesPerSecond) - 1UL;
    private const ulong DisabledMask = 0UL;

    private const ulong ActionUnpressed = 0UL;
    public const ulong ActionPressed = 1UL;
    private const ulong ActionReleased = 2UL;
    private const ulong ActionHeld = 3UL;
    private const ulong DoubleTapUpperMask = ((1UL << (EngineConstants.DoubleTapFrameCountMax - 2)) - 1UL) << 2;

    private ulong _stateMask = EnabledMask;
    private ulong _actionState;

    public ulong ActionState
    {
        get => _actionState;
        set => _actionState = value & _stateMask;
    }

    public void Clear()
    {
        _actionState = 0UL;
    }

    public void UpdateState()
    {
        _actionState = (_actionState << 1) & _stateMask;
    }

    public void SetEnabled(bool enable)
    {
        _stateMask = enable ? EnabledMask : DisabledMask;
    }

    public bool IsActionDown => (_actionState & ActionPressed) == ActionPressed;
    public bool IsActionUp => (_actionState & ActionPressed) == ActionUnpressed;
    public bool IsPressed => (_actionState & ActionHeld) == ActionPressed;
    public bool IsReleased => (_actionState & ActionHeld) == ActionReleased;
    public bool IsHeld => (_actionState & ActionHeld) == ActionHeld;
    public bool IsDoubleTap => (_actionState & ActionPressed) != 0UL &&
                               (_actionState & ActionReleased) == 0UL &&
                               (_actionState & DoubleTapUpperMask) != 0UL;

    public bool IsEnabled => _stateMask != DisabledMask;
}