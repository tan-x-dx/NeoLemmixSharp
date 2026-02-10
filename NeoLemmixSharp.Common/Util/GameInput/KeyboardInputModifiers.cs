namespace NeoLemmixSharp.Common.Util.GameInput;

[Flags]
public enum KeyboardInputModifiers
{
    None = 0,
    Shift = 1 << KeyboardInputModifierHelpers.ShiftShift,
    Ctrl = 1 << KeyboardInputModifierHelpers.CtrlShift,
    Alt = 1 << KeyboardInputModifierHelpers.AltShift,
}

public static class KeyboardInputModifierHelpers
{
    public const int ShiftShift = 0;
    public const int CtrlShift = 1;
    public const int AltShift = 2;

    public static bool ShiftDown(this KeyboardInputModifiers modifiers) => ((((int)modifiers) >> ShiftShift) & 1) != 0;
    public static bool CtrlDown(this KeyboardInputModifiers modifiers) => ((((int)modifiers) >> CtrlShift) & 1) != 0;
    public static bool AltDown(this KeyboardInputModifiers modifiers) => ((((int)modifiers) >> AltShift) & 1) != 0;
}
