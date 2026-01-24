using Microsoft.Xna.Framework.Input;

namespace NeoLemmixSharp.Ui.Components;

internal static class TextInputHandling
{
    public static KeyboardInput GetKeyBoardInput(in KeysEnumerable keys)
    {
        var firstPressedKeyboardInputType = KeyboardInputType.None;
        var firstPressedCharacter = (char)0;
        var shiftPressed = false;

        foreach (var key in keys)
        {
            if (key == Keys.LeftShift || key == Keys.RightShift)
            {
                shiftPressed = true;
                continue;
            }

            KeyboardInputType keyboardInputType = GetKeyboardInputType(key);
            if (firstPressedKeyboardInputType == KeyboardInputType.None)
            {
                firstPressedKeyboardInputType = keyboardInputType;
                if (keyboardInputType == KeyboardInputType.Character)
                {
                    firstPressedCharacter = GetCharFromKey(key);
                }
            }
        }

        if (firstPressedKeyboardInputType == KeyboardInputType.Character && shiftPressed)
        {
            firstPressedCharacter = GetShiftEquivalentChar(firstPressedCharacter);
        }

        return new KeyboardInput(firstPressedKeyboardInputType, firstPressedCharacter);
    }

    private static KeyboardInputType GetKeyboardInputType(Keys key)
    {
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

            _ => KeyboardInputType.Character
        };
    }

    private static char GetCharFromKey(Keys key)
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
            Keys.OemPlus => '+',
            Keys.OemComma => ',',
            Keys.OemMinus => '-',
            Keys.OemPeriod => '.',
            Keys.OemQuestion => '/',
            Keys.OemTilde => '¬',
            Keys.OemOpenBrackets => '[',
            Keys.OemPipe => '|',
            Keys.OemCloseBrackets => ']',
            Keys.OemQuotes => '\'',
            Keys.OemBackslash => '\\',

            _ => (char)0,
        };
    }

    private static char GetShiftEquivalentChar(char c)
    {
        if (c >= 'a' && c <= 'z')
            return (char)(c | 32);

        return c switch
        {
            '0' => ')',
            '1' => '!',
            '2' => '"',
            '3' => '#',
            '4' => '$',
            '5' => '%',
            '6' => '^',
            '7' => '&',
            '8' => '*',
            '9' => '(',

            ';' => ':',
            '\'' => '@',
            '\\' => '|',
            '[' => '{',
            ']' => '}',
            ',' => '<',
            '.' => '>',
            '/' => '?',

            _ => c,
        };
    }
}

internal readonly ref struct KeyboardInput(KeyboardInputType keyboardInputType, char keyboardChar)
{
    public readonly KeyboardInputType KeyboardInputType = keyboardInputType;
    public readonly char KeyboardChar = keyboardChar;
}

internal enum KeyboardInputType
{
    None,

    Character,
    Enter,
    CaretLeft,
    CaretRight,
    CaretStart,
    CaretEnd,
    Backspace,
    Delete,
    Escape
}
