namespace NeoLemmixSharp.Common.Util.GameInput;

public interface IInputAction
{
    ulong ActionState { get; set; }

    public void UpdateState();

    public void SetEnabled(bool enable);

    /// <summary>
    /// Is the Action currently pressed down?
    /// </summary>
    public bool IsActionDown { get; }
    /// <summary>
    /// Is the Action currently released?
    /// </summary>
    public bool IsActionUp { get; }
    /// <summary>
    /// Is the Action currently pressed down, but it was previously released?
    /// </summary>
    public bool IsPressed { get; }
    /// <summary>
    /// Is the Action currently released, but it was previously pressed down?
    /// </summary>
    public bool IsReleased { get; }
    /// <summary>
    /// Is the Action currently being pressed down and it was previously pressed down?
    /// </summary>
    public bool IsHeld { get; }
    /// <summary>
    /// Has a double tap occurred for the Action? Double tap has about 1/4 of a second to take place.
    /// </summary>
    public bool IsDoubleTap { get; }

    public bool IsEnabled { get; }
}