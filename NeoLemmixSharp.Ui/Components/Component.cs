global using KeysEnumerable = NeoLemmixSharp.Common.Util.Collections.SimpleSetEnumerable<NeoLemmixSharp.Common.Util.GameInput.InputController, Microsoft.Xna.Framework.Input.Keys>;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Ui.Components;

public abstract class Component : IDisposable
{
    private int _x, _y;

    private int _width, _height;

    private ColorPacket _colourPacket;

    private string? _textLabel = null;
    private int _labelOffsetX, _labelOffsetY;

    private Component? _parent = null;
    protected List<Component>? _children = null;

    private Action? _clickAction = null;
    private Action? _moveAction = null;
    private Action? _visibilityChangeAction = null;
    private Action? _resizeAction = null;

    private bool _isVisible = true;
    private bool _isDisposed;

    protected Component(int x, int y, int width, int height) : this(x, y, width, height, null) { }

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
        get => _x;
        set
        {
            var oldX = _x;

            _x = value;

            if (_children != null)
            {
                oldX = _x - oldX;

                foreach (Component c in _children)
                {
                    c.Translate(oldX, 0);
                }
            }

            _moveAction?.Invoke();
        }
    }

    public int Top
    {
        get => _y;
        set
        {
            var oldY = _y;

            _y = value;

            if (_children != null)
            {
                oldY = _y - oldY;

                foreach (Component c in _children)
                {
                    c.Translate(0, oldY);
                }
            }

            _moveAction?.Invoke();
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
        var oldX = _x;
        var oldY = _y;

        _x = x;
        _y = y;

        if (_children != null)
        {
            oldX = x - oldX;
            oldY = y - oldY;

            foreach (Component c in _children)
            {
                c.Translate(oldX, oldY);
            }
        }

        _moveAction?.Invoke();
    }

    public void Translate(int dx, int dy)
    {
        _x += dx;
        _y += dy;

        if (_children != null)
        {
            foreach (Component c in _children)
            {
                c.Translate(dx, dy);
            }
        }

        _moveAction?.Invoke();
    }

    public virtual int Width
    {
        get => _width;
        set
        {
            _width = value;

            _resizeAction?.Invoke();
        }
    }

    public virtual int Height
    {
        get => _height;
        set
        {
            _height = value;

            _resizeAction?.Invoke();
        }
    }

    public ColorPacket Colors
    {
        get => _colourPacket;
        set => _colourPacket = value;
    }

    public void SetSize(int w, int h)
    {
        _width = w;
        _height = h;

        _resizeAction?.Invoke();
    }

    public void SetDimensions(int x, int y, int width, int height)
    {
        SetLocation(x, y);
        SetSize(width, height);
    }

    public virtual bool ContainsPoint(LevelPosition position)
    {
        return position.X >= Left &&
               position.Y >= Top &&
               position.X < Right &&
               position.Y < Bottom;
    }

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            var oldVisible = _isVisible;
            _isVisible = value;

            if (oldVisible != _isVisible)
            {
                _visibilityChangeAction?.Invoke();
            }
        }
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

    public void Click() => _clickAction?.Invoke();

    public void Render(SpriteBatch spriteBatch)
    {
        if (_isVisible)
        {
            RenderComponent(spriteBatch);
            RenderLabel(spriteBatch);
        }

        if (_children != null)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Render(spriteBatch);
            }
        }
    }

    protected abstract void RenderComponent(SpriteBatch spriteBatch);

    private void RenderLabel(SpriteBatch spriteBatch)
    {
        if (!string.IsNullOrWhiteSpace(Label))
        {
            var labelPosition = new Vector2(Left + LabelOffsetX, Top + LabelOffsetY);
            spriteBatch.DrawString(
                UiSprites.Font,
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
        if (this == c)
            throw new InvalidOperationException("Cannot add a component to itself [" + ToString() + "]");

        if (c is null)
            throw new ArgumentNullException();

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

    public Component GetTopParent() => _parent == null ? this : _parent.GetTopParent();

    public Component? GetChildAt(LevelPosition position)
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

    public void SetMoveAction(Action? action) => _moveAction = action;
    public void SetClickAction(Action? action) => _clickAction = action;
    public void SetVisibilityChangeAction(Action? action) => _visibilityChangeAction = action;
    public void SetResizeAction(Action action) => _resizeAction = action;

    public virtual void InvokeMouseEnter(LevelPosition mousePosition) { }
    public virtual void InvokeMouseDoubleClick(LevelPosition mousePosition) { }
    public virtual void InvokeMouseDown(LevelPosition mousePosition) => Click();
    public virtual void InvokeMouseUp(LevelPosition mousePosition) { }
    public virtual void InvokeMouseExit(LevelPosition mousePosition) { }

    public virtual void InvokeMouseMovement(LevelPosition mousePosition) { }

    public virtual void InvokeKeyDown(in KeysEnumerable pressedKeys) { }
    public virtual void InvokeKeyUp(in KeysEnumerable pressedKeys) { }

    public void Dispose()
    {
        if (!_isDisposed)
        {
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

            SetMoveAction(null);
            SetClickAction(null);

            SetVisibilityChangeAction(null);

            OnDispose();
            _isDisposed = true;
        }
        GC.SuppressFinalize(this);
    }

    protected virtual void OnDispose()
    {
    }
}
