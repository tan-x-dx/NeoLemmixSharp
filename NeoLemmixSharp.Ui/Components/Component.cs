global using KeysEnumerable = NeoLemmixSharp.Common.Util.Collections.BitArrays.BitArrayEnumerable<NeoLemmixSharp.Common.Util.GameInput.InputController, Microsoft.Xna.Framework.Input.Keys>;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Ui.Data;
using NeoLemmixSharp.Ui.Events;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace NeoLemmixSharp.Ui.Components;

public abstract class Component : IDisposable
{
    public Point Position { get; private set; }
    public Size Dimensions { get; private set; }

    private ComponentState _state = ComponentState.Normal;

    private ColorPacket _colourPacket;

    private string? _textLabel = null;
    private int _labelOffsetX, _labelOffsetY;

    private bool _isVisible = true;
    private bool _isDisposed;

    private Component? _parent = null;
    protected List<Component>? _children = null;

    private MouseEventHandler? _mouseEnter;
    private MouseEventHandler? _mouseMovement;
    private MouseEventHandler? _mouseDown;
    private MouseEventHandler? _mouseDoubleClick;
    private MouseEventHandler? _mouseUp;
    private MouseEventHandler? _mouseExit;

    private KeyboardEventHandler? _keyDown;
    private KeyboardEventHandler? _keyUp;

    public MouseEventHandler MouseEnter => _mouseEnter ??= new MouseEventHandler();
    public MouseEventHandler MouseMovement => _mouseMovement ??= new MouseEventHandler();
    public MouseEventHandler MouseDown => _mouseDown ??= new MouseEventHandler();
    public MouseEventHandler MouseDoubleClick => _mouseDoubleClick ??= new MouseEventHandler();
    public MouseEventHandler MouseUp => _mouseUp ??= new MouseEventHandler();
    public MouseEventHandler MouseExit => _mouseExit ??= new MouseEventHandler();

    public KeyboardEventHandler KeyDown => _keyDown ??= new KeyboardEventHandler();
    public KeyboardEventHandler KeyUp => _keyUp ??= new KeyboardEventHandler();

    protected Component(int x, int y, string? label)
        : this(x, y, UiConstants.TwiceStandardInset + (int)(0.5f + (label?.Length ?? 10) * UiConstants.FontGlyphWidthMultiplier), UiConstants.StandardButtonHeight, label)
    {
    }

    protected Component(int x, int y, int width, int height)
        : this(x, y, width, height, null)
    {
    }

    protected Component(int x, int y, int width, int height, string? label)
    {
        Left = x;
        Top = y;

        Width = width;
        Height = height;
        _colourPacket = UiConstants.RectangularButtonDefaultColours;

        Label = label;
        LabelOffsetX = UiConstants.StandardInset;
        LabelOffsetY = UiConstants.StandardInset;
    }

    public int Left
    {
        get => Position.X;
        set
        {
            var oldX = Position.X;

            Position = new Point(value, Position.Y);

            if (_children != null)
            {
                oldX = Position.X - oldX;

                foreach (Component c in _children)
                {
                    c.Translate(oldX, 0);
                }
            }
        }
    }

    public int Top
    {
        get => Position.Y;
        set
        {
            var oldY = Position.Y;

            Position = new Point(Position.X, value);

            if (_children != null)
            {
                var deltaY = Position.Y - oldY;

                foreach (Component c in _children)
                {
                    c.Translate(0, deltaY);
                }
            }
        }
    }

    public int Right
    {
        get => Left + Width;
        set => Left = value - Width;
    }

    public int Bottom
    {
        get => Top + Height;
        set => Top = value - Height;
    }

    public void SetLocation(int x, int y)
    {
        var oldX = Position.X;
        var oldY = Position.Y;

        Position = new Point(x, y);

        if (_children != null)
        {
            var deltaX = x - oldX;
            var deltaY = y - oldY;

            foreach (Component c in _children)
            {
                c.Translate(deltaX, deltaY);
            }
        }
    }

    public void Translate(int dx, int dy)
    {
        var delta = new Point(dx, dy);
        Position += delta;

        if (_children != null)
        {
            foreach (Component c in _children)
            {
                c.Translate(dx, dy);
            }
        }
    }

    public virtual int Width
    {
        get => Dimensions.W;
        set => Dimensions = new Size(value, Dimensions.H);
    }

    public virtual int Height
    {
        get => Dimensions.H;
        set => Dimensions = new Size(Dimensions.W, value);
    }

    public ColorPacket Colors
    {
        get => _colourPacket;
        set => _colourPacket = value;
    }

    public virtual ComponentState State
    {
        get => _state;
        set => _state = value;
    }

    public void SetSize(int w, int h)
    {
        Dimensions = new Size(w, h);
    }

    public void SetDimensions(int x, int y, int width, int height)
    {
        SetLocation(x, y);
        SetSize(width, height);
    }

    public virtual bool ContainsPoint(Point position)
    {
        return position.X >= Left &&
               position.Y >= Top &&
               position.X < Right &&
               position.Y < Bottom;
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => _isVisible = value;
    }

    public virtual string? Label
    {
        get => _textLabel;
        set => _textLabel = value;
    }

    public int LabelOffsetX
    {
        get => _labelOffsetX;
        set => _labelOffsetX = value;
    }

    public int LabelOffsetY
    {
        get => _labelOffsetY;
        set => _labelOffsetY = value;
    }

    public virtual void Render(SpriteBatch spriteBatch)
    {
        if (_isVisible)
            RenderComponent(spriteBatch);
        RenderChildren(spriteBatch);
    }

    protected void RenderChildren(SpriteBatch spriteBatch)
    {
        if (_children != null)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Render(spriteBatch);
            }
        }
    }

    protected virtual void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawBeveledRectangle(spriteBatch, this);
        RenderLabel(spriteBatch);
    }

    private void RenderLabel(SpriteBatch spriteBatch)
    {
        if (!string.IsNullOrWhiteSpace(Label))
        {
            var labelPosition = new Vector2(Left + LabelOffsetX, Top + LabelOffsetY);
            spriteBatch.DrawString(
                UiSprites.UiFont,
                _textLabel,
                labelPosition,
                Color.White,
                0f,
                Vector2.Zero,
                UiConstants.FontScaleFactor,
                SpriteEffects.None,
                1.0f);
        }
    }

    public void AddComponent(Component? c) => AddComponent(c, -1);

    public void AddComponent(Component? c, int index)
    {
        ArgumentNullException.ThrowIfNull(c);

        if (this == c)
            throw new InvalidOperationException("Cannot add a component to itself [" + ToString() + "]");

        if (c._parent is not null)
            throw new InvalidOperationException("Component is already a child [" + c.ToString() + "]");

        _children ??= new List<Component>();

        if (index < 0)
        {
            _children.Add(c);
        }
        else
        {
            _children.Insert(index, c);
        }

        c._parent = this;

        c.Translate(Left, Top);
    }

    public bool IsChild() => _parent != null;

    public bool HasChildren() => _children != null;

    public Component? GetParent() => _parent;

    public Component GetTopParent()
    {
        Component parent = this;

        while (true)
        {
            var higherParent = parent.GetParent();
            if (higherParent is null)
                return parent;
            parent = higherParent;
        }
    }

    public Component? GetChildAt(Point position)
    {
        if (_children != null)
        {
            foreach (Component child in _children)
            {
                if (child.IsVisible && child.ContainsPoint(position))
                {
                    return child.GetChildAt(position);
                }
            }
        }

        return ContainsPoint(position) ? this : null;
    }

    public void InvokeMouseEnter(Point mousePosition) => _mouseEnter?.Invoke(this, mousePosition);
    public void InvokeMouseMovement(Point mousePosition) => _mouseMovement?.Invoke(this, mousePosition);
    public void InvokeMouseDown(Point mousePosition) => _mouseDown?.Invoke(this, mousePosition);
    public void InvokeMouseDoubleClick(Point mousePosition) => _mouseDoubleClick?.Invoke(this, mousePosition);
    public void InvokeMouseUp(Point mousePosition) => _mouseUp?.Invoke(this, mousePosition);
    public void InvokeMouseExit(Point mousePosition) => _mouseExit?.Invoke(this, mousePosition);

    public void InvokeKeyDown(in KeysEnumerable pressedKeys) => _keyDown?.Invoke(this, in pressedKeys);
    public void InvokeKeyUp(in KeysEnumerable pressedKeys) => _keyUp?.Invoke(this, in pressedKeys);

    protected void SetMouseOver(Component c, Point p) => State = ComponentState.MouseOver;
    protected void SetMousePress(Component c, Point p) => State = ComponentState.MousePress;
    protected void SetMouseNormal(Component c, Point p) => State = ComponentState.Normal;

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;

            if (_children is not null)
            {
                foreach (Component child in _children)
                {
                    child.Dispose();
                }

                _children.Clear();
                _children = null;
            }

            _parent = null;

            _mouseEnter?.Clear();
            _mouseMovement?.Clear();
            _mouseDown?.Clear();
            _mouseDoubleClick?.Clear();
            _mouseUp?.Clear();
            _mouseExit?.Clear();

            _keyDown?.Clear();
            _keyUp?.Clear();

            OnDispose();
        }
        GC.SuppressFinalize(this);
    }

    protected virtual void OnDispose()
    {
    }
}
