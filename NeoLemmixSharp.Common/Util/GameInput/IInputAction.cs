namespace NeoLemmixSharp.Common.Util.GameInput;

public interface IInputAction
{
    ulong ActionState { get; set; }

    void Clear();

    void UpdateState();

    void SetEnabled(bool enable);

    /// <summary>
    /// Is the Action currently pressed down?
    /// </summary>
    bool IsActionDown { get; }
    /// <summary>
    /// Is the Action currently released?
    /// </summary>
    bool IsActionUp { get; }
    /// <summary>
    /// Is the Action currently pressed down, but it was previously released?
    /// </summary>
    bool IsPressed { get; }
    /// <summary>
    /// Is the Action currently released, but it was previously pressed down?
    /// </summary>
    bool IsReleased { get; }
    /// <summary>
    /// Is the Action currently being pressed down, and it was previously pressed down?
    /// </summary>
    bool IsHeld { get; }
    /// <summary>
    /// Has a double tap occurred for the Action? Double tap has about 1/4 of a second to take place.
    /// </summary>
    bool IsDoubleTap { get; }

    bool IsEnabled { get; }
}