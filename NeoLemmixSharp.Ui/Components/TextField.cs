using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.GameInput;
using NeoLemmixSharp.Ui.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Ui.Components;

public sealed class TextField : Component
{
    private const int TextXOffset = 4;
    private const int TextYOffset = 6;

    private const int CaretBlinkShift = 5;
    private const int CaretBlinkDelay = 1 << CaretBlinkShift;
    private const int CaretBlinkMask = (1 << (CaretBlinkShift + 1)) - 1;

    private string _currentString = string.Empty;
    private string? _charMask;
    private char[] _charBuffer = [];

    private int _currentStringLength;
    private int _caretPosition;
    private int _caretFlashCount;
    private int _caretPhysicalOffset;

    private bool _isSelected;

    private int CaretPosition
    {
        get => _caretPosition;
        set
        {
            if (_caretPosition == value)
                return;

            _caretPosition = value;
            _caretPhysicalOffset = CalculateCaretPhysicalOffset();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
                return;

            _isSelected = value;
            CaretPosition = 0;
            _caretFlashCount = 0;
        }
    }

    public ReadOnlySpan<char> CurrentTextSpan => Helpers.CreateReadOnlySpan(_charBuffer, 0, _currentStringLength);
    public string CurrentString => CurrentString.ToString();
    public bool TryParseInt(out int value) => int.TryParse(CurrentTextSpan, out value);

    public TextField(int x, int y) : base(x, y)
    {
        KeyPressed.RegisterKeyEvent(HandleKeyDown);

        Colors = new ColorPacket(
            Color.Wheat,
            0xff666666.AsAbgrColor(),
            0xff888888.AsAbgrColor(),
            0xff006600.AsAbgrColor());
    }

    public void SetTextMask(string? charMask)
    {
        _charMask = charMask;
        Clear();
    }

    public void SetCapacity(int capacity)
    {
        Array.Resize(ref _charBuffer, capacity);
    }

    public void SetText(string newText)
    {
        CaretPosition = 0;
        var clippedTextLength = Math.Min(_charBuffer.Length, newText.Length);
        var newTextSpan = newText.AsSpan(0, clippedTextLength);
        var span = new Span<char>(_charBuffer);

        newTextSpan.CopyTo(span);
        _currentStringLength = clippedTextLength;
        OnStringContentsChanged();
    }

    public void Clear()
    {
        new Span<char>(_charBuffer).Clear();
        CaretPosition = 0;
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
        var keyboardInput = UiHandler.Instance.InputController.LatestKeyboardInput();

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

        var c = keyboardInput.GetCorrespondingChar();

        if (!MaskAllowsCharacter(c))
            return;

        var i = _currentStringLength;
        _currentStringLength++;
        while (i > CaretPosition)
        {
            _charBuffer.At(i) = _charBuffer.At(i - 1);
            i--;
        }

        _charBuffer.At(CaretPosition) = c;
        OnStringContentsChanged();
        MoveCaret(1);
    }

    private bool MaskAllowsCharacter(char c)
    {
        return _charMask is null || _charMask.Contains(c);
    }

    public void MoveCaret(int delta) => SetCaretPosition(CaretPosition + delta);

    public void SetCaretPosition(int newCaretPosition)
    {
        if (newCaretPosition < 0)
        {
            CaretPosition = 0;
        }
        else if (newCaretPosition > _currentStringLength)
        {
            CaretPosition = _currentStringLength;
        }
        else
        {
            CaretPosition = newCaretPosition;
        }
    }

    public void HandleEnter()
    {
    }

    public void HandleBackspace()
    {
        if (CaretPosition == 0)
            return;

        var i = CaretPosition - 1;
        var lastCharIndex = _currentStringLength - 1;
        while (i < lastCharIndex)
        {
            _charBuffer.At(i) = _charBuffer.At(i + 1);
            i++;
        }

        _charBuffer.At(lastCharIndex) = (char)0;
        _currentStringLength--;
        OnStringContentsChanged();
        MoveCaret(-1);
    }

    private void OnStringContentsChanged()
    {
        var currentStringSpan = _currentString.AsSpan();
        var currentTextSpan = CurrentTextSpan;

        if (currentTextSpan.Equals(currentStringSpan, StringComparison.Ordinal))
            return;

        _currentString = currentTextSpan.ToString();
    }

    public void HandleDelete()
    {
        if (CaretPosition == _currentStringLength)
            return;

        MoveCaret(1);
        HandleBackspace();
    }

    public void Deselect()
    {
        UiHandler.Instance.DeselectTextField();
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        var caretFlashCount = _caretFlashCount;
        caretFlashCount++;
        caretFlashCount &= CaretBlinkMask;
        _caretFlashCount = caretFlashCount;

        UiSprites.DrawBeveledRectangle(spriteBatch, this);

        var textPosition = new Vector2(Left + TextXOffset, Top + TextYOffset);
        spriteBatch.DrawString(
             UiSprites.UiFont,
             _currentString,
             textPosition,
             Color.Black);

        if (!IsSelected)
            return;
        if (caretFlashCount >= CaretBlinkDelay)
            return;

        var rect = new Rectangle(Left + TextXOffset, Top, 1, Height);
        rect.X += _caretPhysicalOffset;
        rect.Y += 4;
        rect.Height -= 10;

        spriteBatch.FillRect(
            rect,
            Color.Black);
    }

    private int CalculateCaretPhysicalOffset()
    {
        if (CaretPosition == 0)
            return 0;

        var characterGlyphs = UiSprites.UiFontGlyphs;
        var currentTextSpan = Helpers.CreateReadOnlySpan(_charBuffer, 0, CaretPosition);

        float caretPhysicalOffset = 0f;
        float zero = 0f;
        bool startOfText = true;
        foreach (var c in currentTextSpan)
        {
            ref var glyph = ref CollectionsMarshal.GetValueRefOrNullRef(characterGlyphs, c);

            if (startOfText)
            {
                zero = Math.Max(glyph.LeftSideBearing, 0f);
                startOfText = false;
            }
            else
            {
                zero += UiSprites.UiFont.Spacing + glyph.LeftSideBearing;
            }

            zero += glyph.Width;
            float charBearingOffset = zero + Math.Max(glyph.RightSideBearing, 0f);
            if (charBearingOffset > caretPhysicalOffset)
            {
                caretPhysicalOffset = charBearingOffset;
            }

            zero += glyph.RightSideBearing;
        }

        caretPhysicalOffset -= 1f;
        return (int)caretPhysicalOffset;
    }
}
