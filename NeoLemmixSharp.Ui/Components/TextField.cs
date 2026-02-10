using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.GameInput;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Ui.Components;

public sealed class TextField : Component
{
    private const int CaretBlinkShift = 5;
    private const int CaretBlinkDelay = 1 << CaretBlinkShift;
    private const int CaretBlinkMask = (1 << (CaretBlinkShift + 1)) - 1;

    private readonly UiHandler _uiHandler;

    private char[] _charBuffer = [];
    private string _currentString = string.Empty;
    private int _currentStringLength;
    private int _caretPosition;
    private int _caretFlashCount;

    public bool IsSelected
    {
        get => field;
        set
        {
            if (field == value)
                return;

            field = value;
            _caretPosition = 0;
            _caretFlashCount = 0;
        }
    }

    public TextField(UiHandler uiHandler, int x, int y, string label) : base(x, y, label)
    {
        _uiHandler = uiHandler;
        KeyPressed.RegisterKeyEvent(HandleKeyDown);
    }

    public void SetCapacity(int capacity)
    {
        Array.Resize(ref _charBuffer, capacity);
    }

    public void SetText(string newText)
    {
        _caretPosition = 0;
        var clippedTextLength = Math.Min(_charBuffer.Length, newText.Length);
        var newTextSpan = newText.AsSpan(0, clippedTextLength);
        var span = new Span<char>(_charBuffer);

        newTextSpan.CopyTo(span);

        _currentString = clippedTextLength == newText.Length
            ? newText
            : newTextSpan.ToString();
        _currentStringLength = clippedTextLength;
    }

    public void Clear()
    {
        new Span<char>(_charBuffer).Clear();
        _caretPosition = 0;
        _currentString = string.Empty;
        _currentStringLength = 0;
    }

    private void HandleKeyDown(Component _, in KeysEnumerable keys)
    {
        if (!IsSelected)
            return;

        if (keys.Count > 0)
            EvaluatePressedKey();
    }

    private void EvaluatePressedKey()
    {
        var keyboardInput = _uiHandler.InputController.LatestKeyboardInput();

        var numberOfFramesThisKeyHasBeenPressed = keyboardInput.NumberOfFramesThisKeyHasBeenPressed;
        if (numberOfFramesThisKeyHasBeenPressed > 1 &&
            numberOfFramesThisKeyHasBeenPressed < UiConstants.KeyboardInputFrameDelay)
            return;

        // Slow down the inputs just a bit...
        if ((numberOfFramesThisKeyHasBeenPressed & 1) == 0)
            return;

        switch (keyboardInput.KeyboardInputType)
        {
            case KeyboardInputType.None:
                break;

            case KeyboardInputType.Character:
                HandleCharacter(keyboardInput);
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
                SetCaretPosition(0);
                break;

            case KeyboardInputType.CaretEnd:
                SetCaretPosition(_currentStringLength);
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

    public void HandleCharacter(KeyboardInput keyboardInput)
    {
        if (_currentStringLength == _charBuffer.Length)
            return;

        var i = _currentStringLength;
        _currentStringLength++;
        while (i >= _caretPosition)
        {
            _charBuffer.At(i + 1) = _charBuffer.At(i);
            i--;
        }

        _charBuffer.At(_caretPosition) = keyboardInput.GetCorrespondingChar();
        MoveCaret(1);
        UpdateCurrentString();
    }

    public void MoveCaret(int delta) => SetCaretPosition(_caretPosition + delta);

    public void SetCaretPosition(int newCaretPosition)
    {
        if (newCaretPosition < 0)
        {
            _caretPosition = 0;
        }
        else if (newCaretPosition > _currentStringLength)
        {
            _caretPosition = _currentStringLength;
        }
        else
        {
            _caretPosition = newCaretPosition;
        }
    }

    public void HandleEnter()
    {
    }

    public void HandleBackspace()
    {
        if (_caretPosition == 0)
            return;

        var i = _caretPosition - 1;
        var lastCharIndex = _currentStringLength - 1;
        while (i < lastCharIndex)
        {
            _charBuffer.At(i) = _charBuffer.At(i + 1);
            i++;
        }

        _charBuffer.At(lastCharIndex) = (char)0;
        _currentStringLength--;
        MoveCaret(-1);
        UpdateCurrentString();
    }

    public void HandleDelete()
    {
        if (_caretPosition == _currentStringLength)
            return;

        MoveCaret(1);
        HandleBackspace();
    }

    public void Deselect()
    {
        _uiHandler.DeselectTextField();
    }

    private void UpdateCurrentString()
    {
        var currentStringSpan = _currentString.AsSpan();
        var newStringSpan = Helpers.CreateReadOnlySpan(_charBuffer, 0, _currentStringLength);

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

        //   var textPosition = new Vector2(rect.X + 2, rect.Y + 2);

        var newStringSpan = Helpers.CreateReadOnlySpan(_charBuffer, 0, _currentStringLength);
        FontBank.MenuFont.RenderText(
            spriteBatch,
            newStringSpan,
            rect.X + 2,
            rect.Y + 6,
            1,
            EngineConstants.PanelBlue);
        /* spriteBatch.DrawString(
             UiSprites.UiFont,
             _currentString,
             textPosition,
             Color.White);*/

        if (!IsSelected)
            return;
        if (caretFlashCount >= CaretBlinkDelay)
            return;

        rect.Y += 2;
        rect.Width = 1;
        rect.Height -= 4;

        const int charWidth = MenuFont.GlyphWidth;
        rect.X = Left + 2 + (_caretPosition * charWidth);

        spriteBatch.FillRect(
            rect,
            Color.White);
    }
}
