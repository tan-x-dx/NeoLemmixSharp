using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.GameInput;
using NeoLemmixSharp.Ui.Data;
using NeoLemmixSharp.Ui.Events;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Ui.Components;

public sealed class TextField : Component
{
    private const int CaretBlinkShift = 5;
    private const int CaretBlinkDelay = 1 << CaretBlinkShift;
    private const int CaretBlinkMask = (1 << (CaretBlinkShift + 1)) - 1;

    private string _currentString = string.Empty;
    private string? _charMask;
    private char[] _previousContentsCharBuffer = [];
    private char[] _currentContentsCharBuffer = [];

    private GenericEventHandler? _textSubmit;

    public int TextXOffset { get; set; } = UiConstants.DefaultTextXOffset;
    public int TextYOffset { get; set; } = UiConstants.DefaultTextYOffset;

    private int _previousStringLength;
    private int _currentStringLength;
    private int _caretPosition;
    private int _caretFlashCount;
    private int _caretPhysicalOffset;

    private bool _isSelected;

    public GenericEventHandler TextSubmit => _textSubmit ??= new GenericEventHandler();

    private int CaretPosition
    {
        get => _caretPosition;
        set
        {
            if (_caretPosition == value)
                return;

            _caretPosition = value;

            var caretPhysicalOffset = 0;
            if (value > 0)
            {
                var currentTextSpan = Helpers.CreateReadOnlySpan(_currentContentsCharBuffer, 0, CaretPosition);
                caretPhysicalOffset = CalculateCaretPhysicalOffset(currentTextSpan);
            }
            _caretPhysicalOffset = caretPhysicalOffset;
        }
    }

    public bool IsSelected => _isSelected;

    public bool AutoCapitaliseLetters { get; set; }

    public void InvokeTextSubmit() => _textSubmit?.Invoke(this);

    public ReadOnlySpan<char> CurrentTextSpan => Helpers.CreateReadOnlySpan(_currentContentsCharBuffer, 0, _currentStringLength);
    public string CurrentString => _currentString;
    public bool IsBlank() => _currentStringLength == 0;
    public int ParseInt() => int.Parse(CurrentTextSpan);
    public bool TryParseInt(out int value) => int.TryParse(CurrentTextSpan, out value);

    public TextField() : this(0, 0)
    {
    }

    public TextField(int x, int y) : base(x, y)
    {
        KeyPressed.RegisterKeyEvent(HandleKeyDown);

        Colors = new ColorPacket(
            Color.WhiteSmoke,
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
        Array.Resize(ref _currentContentsCharBuffer, capacity);
        Array.Resize(ref _previousContentsCharBuffer, capacity);
        Clear();
    }

    public void SetText(string newText)
    {
        SetText(newText.AsSpan());
    }

    public void SetText(ReadOnlySpan<char> newText)
    {
        CaretPosition = 0;
        var clippedTextLength = Math.Min(_currentContentsCharBuffer.Length, newText.Length);
        var newTextSpan = newText.SliceUnsafe(0, clippedTextLength);

        var span = new Span<char>(_currentContentsCharBuffer);
        newTextSpan.CopyTo(span);
        span = new Span<char>(_previousContentsCharBuffer);
        newTextSpan.CopyTo(span);

        _currentStringLength = clippedTextLength;
        _previousStringLength = clippedTextLength;
        OnStringContentsChanged();
    }

    public void Clear()
    {
        new Span<char>(_previousContentsCharBuffer).Clear();
        new Span<char>(_currentContentsCharBuffer).Clear();
        CaretPosition = 0;
        _previousStringLength = 0;
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
                InvokeTextSubmit();
                Deselect();
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
                Revert();
                Deselect();
                break;

            default:
                Deselect();
                break;
        }
    }

    public void HandleCharacter(KeyboardInput keyboardInput)
    {
        if (_currentStringLength == _currentContentsCharBuffer.Length)
            return;

        var c = keyboardInput.GetCorrespondingChar();

        if (AutoCapitaliseLetters)
            c = char.ToUpper(c);

        if (!MaskAllowsCharacter(c))
            return;

        var i = _currentStringLength;
        _currentStringLength++;
        while (i > CaretPosition)
        {
            _currentContentsCharBuffer.At(i) = _currentContentsCharBuffer.At(i - 1);
            i--;
        }

        _currentContentsCharBuffer.At(CaretPosition) = c;
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
            newCaretPosition = 0;
        }
        else if (newCaretPosition > _currentStringLength)
        {
            newCaretPosition = _currentStringLength;
        }
        CaretPosition = newCaretPosition;
    }

    public void HandleBackspace()
    {
        if (CaretPosition == 0)
            return;

        var i = CaretPosition - 1;
        var lastCharIndex = _currentStringLength - 1;
        while (i < lastCharIndex)
        {
            _currentContentsCharBuffer.At(i) = _currentContentsCharBuffer.At(i + 1);
            i++;
        }

        _currentContentsCharBuffer.At(lastCharIndex) = (char)0;
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

    private void Revert()
    {
        var newTextBuffer = new Span<char>(_currentContentsCharBuffer);
        var previousTextBuffer = new ReadOnlySpan<char>(_previousContentsCharBuffer);
        previousTextBuffer.CopyTo(newTextBuffer);
        _currentStringLength = _previousStringLength;

        OnStringContentsChanged();
    }

    public void SetSelected()
    {
        if (_isSelected)
            return;

        _isSelected = true;
        CaretPosition = 0;
        _caretFlashCount = 0;
    }

    public void Deselect()
    {
        if (!_isSelected)
            return;

        _isSelected = false;
        CaretPosition = 0;
        _caretFlashCount = 0;

        var newTextBuffer = new ReadOnlySpan<char>(_currentContentsCharBuffer);
        var previousTextBuffer = new Span<char>(_previousContentsCharBuffer);
        newTextBuffer.CopyTo(previousTextBuffer);
        _previousStringLength = _currentStringLength;
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

    internal void SetCaretPositionFromMousePosition(Common.Point mousePosition)
    {
        var localXPosition = mousePosition.X - Left - TextXOffset;
        CaretPosition = CalculateCaretPosition(CurrentTextSpan, localXPosition);
    }

    private static int CalculateCaretPhysicalOffset(ReadOnlySpan<char> currentTextSpan)
    {
        var characterGlyphs = UiSprites.UiFontGlyphs;

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

    private static int CalculateCaretPosition(ReadOnlySpan<char> currentTextSpan, float localXPosition)
    {
        var characterGlyphs = UiSprites.UiFontGlyphs;

        float caretPhysicalOffset = 0f;
        float zero = 0f;
        bool startOfText = true;
        var i = 0;
        for (; i < currentTextSpan.Length; i++)
        {
            var c = currentTextSpan[i];
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

            if (caretPhysicalOffset >= localXPosition)
                break;

            zero += glyph.RightSideBearing;
        }

        if (caretPhysicalOffset < localXPosition)
            return currentTextSpan.Length;

        return i;
    }

    protected override void OnDispose()
    {
        DisposableHelperMethods.DisposeOf(ref _textSubmit);
    }
}
