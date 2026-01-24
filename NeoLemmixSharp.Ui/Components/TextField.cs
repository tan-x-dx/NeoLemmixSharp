using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Ui.Components;

public sealed class TextField : Component
{
    private const int CaretBlinkShift = 4;
    private const int CaretBlinkDelay = 1 << CaretBlinkShift;
    private const int CaretBlinkMask = (1 << (CaretBlinkShift + 1)) - 1;

    private readonly UiHandler _uiHandler;

    private char[] _charBuffer = [];
    private string _currentString = string.Empty;
    private int _caretPosition;
    private int _stringLength;
    private int _caretFlashCount;

    public bool IsSelected
    {
        get => field;
        set
        {
            field = value;
            _caretFlashCount = 0;
        }
    }

    public TextField(UiHandler uiHandler, int x, int y, string label) : base(x, y, label)
    {
        _uiHandler = uiHandler;

        KeyDown.RegisterMouseEvent(HandleKeyDown);
        Width = 120;
    }

    public void SetCapacity(int capacity)
    {
        Array.Resize(ref _charBuffer, capacity);
    }

    public void SetText(string text)
    {
        _caretPosition = 0;
        var textLength = text.Length;
        var textSpan = text.AsSpan(0, _charBuffer.Length);
        var span = new Span<char>(_charBuffer);

        textSpan.CopyTo(span);
        _stringLength = textLength;

        textSpan = Helpers.CreateReadOnlySpan(_charBuffer, 0, textLength);
        _currentString = textSpan.ToString();
    }

    public void Clear()
    {
        new Span<char>(_charBuffer).Clear();
        _stringLength = 0;
        _caretPosition = 0;
        _currentString = string.Empty;
    }

    private void HandleKeyDown(Component _, in KeysEnumerable keys)
    {
        if (!IsSelected)
            return;

        EvaluatePressedKey(in keys);
    }

    private void EvaluatePressedKey(in KeysEnumerable keys)
    {
        if (keys.Count == 0)
            return;

        var input = TextInputHandling.GetKeyBoardInput(in keys);
        switch (input.KeyboardInputType)
        {
            case KeyboardInputType.None:
                break;

            case KeyboardInputType.Character:
                HandleCharacter(input.KeyboardChar);
                break;

            case KeyboardInputType.Enter:
                HandleEnter();
                break;

            case KeyboardInputType.CaretLeft:
                MoveCaret(-1);
                break;

            case KeyboardInputType.CaretRight:
                MoveCaret(1);
                break;

            case KeyboardInputType.CaretStart:
                MoveCaret(-_charBuffer.Length);
                break;

            case KeyboardInputType.CaretEnd:
                MoveCaret(_charBuffer.Length);
                break;

            case KeyboardInputType.Backspace:
                HandleBackspace();
                break;

            case KeyboardInputType.Delete:
                HandleDelete();
                break;

            case KeyboardInputType.Escape:
                Deselect();
                break;
            default:
                Deselect();
                break;
        }
    }

    private void HandleCharacter(char keyboardChar)
    {
        if (_stringLength == _charBuffer.Length)
            return;

        var i = _stringLength - 1;
        while (i >= _caretPosition)
        {
            _charBuffer[i + 1] = _charBuffer[i];
            i--;
        }

        _charBuffer[_caretPosition] = keyboardChar;
        _stringLength++;
        MoveCaret(1);
        UpdateCurrentString();
    }

    private void MoveCaret(int delta)
    {
        var newCaretPosition = _caretPosition + delta;

        if (newCaretPosition < 0)
        {
            _caretPosition = 0;
        }
        else if (newCaretPosition >= _stringLength)
        {
            _caretPosition = _stringLength;
        }
        else
        {
            _caretPosition = newCaretPosition;
        }
    }

    private void HandleEnter()
    {
        throw new NotImplementedException();
    }

    private void HandleBackspace()
    {
        throw new NotImplementedException();
    }

    private void HandleDelete()
    {
        throw new NotImplementedException();
    }

    private void Deselect()
    {
        _uiHandler.DeselectTextField();
    }

    private void UpdateCurrentString()
    {
        var currentStringSpan = _currentString.AsSpan();
        var newStringSpan = Helpers.CreateReadOnlySpan(_charBuffer, 0, _stringLength);

        if (currentStringSpan.Equals(newStringSpan, StringComparison.Ordinal))
            return;

        _currentString = newStringSpan.ToString();
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        var caretFlashCount = _caretFlashCount;
        caretFlashCount++;
        caretFlashCount &= CaretBlinkMask;
        _caretFlashCount = caretFlashCount;

        var rect = new Rectangle(Left, Top, Width, Height);

        spriteBatch.FillRect(
            rect,
            Color.Red);

        var textPosition = new Vector2(rect.X, rect.Y);
        spriteBatch.DrawString(
            UiSprites.UiFont,
            _currentString,
            textPosition,
            Color.White);

        if (!IsSelected)
            return;
        if (caretFlashCount >= CaretBlinkDelay)
            return;

        rect.Y += 2;
        rect.Width = 1;
        rect.Height -= 4;

        const int charWidth = 8;
        rect.X = Left + 2 + (_caretPosition * charWidth);

        spriteBatch.FillRect(
            rect,
            Color.White);
    }
}
