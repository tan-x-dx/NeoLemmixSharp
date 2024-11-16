using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Ui.Components;

public abstract class Component
{
    public delegate void Action();


    private int _x, _y;

    private string? _textLabel = null;
    private int _labelOffsetX, _labelOffsetY;

    private bool _visible = true;

    private Component? _parent = null;
    private List<Component>? _children = null;

    private Action? _clickAction = null;
    private Action? _redrawAction = null;
    private Action? _moveAction = null;
    private Action? _visibilityChangeAction = null;

    protected Component() : this(0, 0, null) { }

    protected Component(int x, int y) : this(x, y, null) { }

    protected Component(int x, int y, string? label)
    {
        Left = x;
        Top = y;

        Label = label;
        LabelOffsetX = UiConstants.StandardInset;
        LabelOffsetY = UiConstants.StandardInset;
    }

    public int Left
    {
        get { return _x; }
        set
        {
            int oldX = _x;

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
        get { return _y; }
        set
        {
            int oldY = _y;

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
        get { return Left + Width; }
        set { Left = value - Width; }
    }

    public int Bottom
    {
        get { return Top + Height; }
        set { Top = value - Height; }
    }

    public void SetLocation(int x, int y)
    {
        int oldX = _x;
        int oldY = _y;

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

    public abstract int Width { get; set; }
    public abstract int Height { get; set; }

    public bool Visible
    {
        get => _visible;
        set
        {
            bool oldVisible = _visible;
            _visible = value;

            if (oldVisible != _visible)
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
        RenderComponent(spriteBatch);
        RenderLabel(spriteBatch);

        _redrawAction?.Invoke();

        if (_children != null)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                Component c = _children[i];
                if (c.Visible)
                {
                    c.Render(spriteBatch);
                }
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

    public void AddComponent(Component c) => AddComponent(c, -1);

    public void AddComponent(Component c, int index)
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

    public abstract bool ContainsPoint(LevelPosition position);

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
                if (child.Visible && child.ContainsPoint(position))
                {
                    return child.GetChildAt(position);
                }
            }
        }

        return ContainsPoint(position) ? this : null;
    }

    public void SetMoveAction(Action action) => _moveAction = action;

    public void SetRedrawAction(Action action) => _redrawAction = action;

    public void SetClickAction(Action action) => _clickAction = action;

    public void SetVisibilityChangeAction(Action action) => _visibilityChangeAction = action;

    public virtual void InvokeMouseEnter(LevelPosition mousePosition) { }
    public virtual void InvokeMouseDown(LevelPosition mousePosition) => Click();
    public virtual void InvokeMouseUp(LevelPosition mousePosition) { }
    public virtual void InvokeMouseExit(LevelPosition mousePosition) { }

    public virtual void InvokeMouseMovement(LevelPosition mousePosition) { }

    public virtual void InvokeKeyDown() { }
    public virtual void InvokeKeyUp() { }
}
