using Microsoft.Xna.Framework.Input;

namespace NeoLemmixSharp.Common.Util.GameInput;

public static class KeyboardHelpers
{
    public static KeyboardInputType GetKeyboardInputType(Keys key)
    {
        if (key >= Keys.A && key <= Keys.Z)
            return KeyboardInputType.Character;

        if (key >= Keys.D0 && key <= Keys.D9)
            return KeyboardInputType.Character;

        if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
            return KeyboardInputType.Character;

        return key switch
        {
            Keys.Back => KeyboardInputType.Backspace,
            Keys.Enter => KeyboardInputType.Enter,
            Keys.Escape => KeyboardInputType.Escape,
            Keys.End => KeyboardInputType.CaretEnd,
            Keys.Home => KeyboardInputType.CaretStart,
            Keys.Left => KeyboardInputType.CaretLeft,
            Keys.Right => KeyboardInputType.CaretRight,
            Keys.Delete => KeyboardInputType.Delete,

            Keys.Space => KeyboardInputType.Character,
            Keys.Multiply => KeyboardInputType.Character,
            Keys.Add => KeyboardInputType.Character,
            Keys.Separator => KeyboardInputType.Character,
            Keys.Subtract => KeyboardInputType.Character,
            Keys.Decimal => KeyboardInputType.Character,
            Keys.Divide => KeyboardInputType.Character,

            Keys.OemSemicolon => KeyboardInputType.Character,
            Keys.OemPlus => KeyboardInputType.Character,
            Keys.OemComma => KeyboardInputType.Character,
            Keys.OemMinus => KeyboardInputType.Character,
            Keys.OemPeriod => KeyboardInputType.Character,
            Keys.OemQuestion => KeyboardInputType.Character,
            Keys.OemTilde => KeyboardInputType.Character,
            Keys.OemOpenBrackets => KeyboardInputType.Character,
            Keys.OemPipe => KeyboardInputType.Character,
            Keys.OemCloseBrackets => KeyboardInputType.Character,
            Keys.OemQuotes => KeyboardInputType.Character,
            Keys.OemBackslash => KeyboardInputType.Character,

            _ => KeyboardInputType.None
        };
    }

    public static char GetCharFromKey(Keys key)
    {
        if (key >= Keys.A && key <= Keys.Z)
        {
            var keyAsInt = key - Keys.A;
            keyAsInt += 'a';
            return (char)keyAsInt;
        }

        if (key >= Keys.D0 && key <= Keys.D9)
        {
            var keyAsInt = key - Keys.D0;
            keyAsInt += '0';
            return (char)keyAsInt;
        }

        if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
        {
            var keyAsInt = key - Keys.NumPad0;
            keyAsInt += '0';
            return (char)keyAsInt;
        }

        return key switch
        {
            Keys.Space => ' ',
            Keys.Multiply => '*',
            Keys.Add => '+',
            Keys.Separator => '/',
            Keys.Subtract => '-',
            Keys.Decimal => '.',
            Keys.Divide => '/',

            Keys.OemSemicolon => ';',
            Keys.OemPlus => '=',
            Keys.OemComma => ',',
            Keys.OemMinus => '-',
            Keys.OemPeriod => '.',
            Keys.OemQuestion => '/',
            Keys.OemTilde => '~',
            Keys.OemOpenBrackets => '[',
            Keys.OemPipe => '\\',
            Keys.OemCloseBrackets => ']',
            Keys.OemQuotes => '\'',
            Keys.OemBackslash => '\\',

            _ => (char)0,
        };
    }
}
