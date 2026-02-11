namespace NeoLemmixSharp.Common.Util.GameInput;

public readonly ref struct KeyboardInput(KeyboardInputType keyboardInputType, KeyboardInputModifiers inputModifiers, int numberOfFramesThisKeyHasBeenPressed, char keyboardChar, bool capslock)
{
    public readonly KeyboardInputType KeyboardInputType = keyboardInputType;
    public readonly KeyboardInputModifiers InputModifiers = inputModifiers;
    public readonly int NumberOfFramesThisKeyHasBeenPressed = numberOfFramesThisKeyHasBeenPressed;
    private readonly char _keyboardChar = keyboardChar;
    private readonly bool _capslock = capslock;

    public char GetCorrespondingChar()
    {
        var shiftDown = InputModifiers.ShiftDown();

        var c = _keyboardChar;
        if (c >= 'a' && c <= 'z')
        {
            var mask = -1;
            if (_capslock ^ shiftDown)
                mask = ~32;
            return (char)(c & mask);
        }

        if (shiftDown)
            return GetShiftEquivalentChar(c);

        return _keyboardChar;
    }

    private static char GetShiftEquivalentChar(char c) => c switch
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
        '-' => '_',
        '=' => '+',
        '#' => '~',

        _ => c,
    };
}
