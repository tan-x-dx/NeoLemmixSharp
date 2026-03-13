global using KeysEnumerable = NeoLemmixSharp.Common.Util.Collections.BitArrays.BitArrayEnumerable<NeoLemmixSharp.Common.Util.GameInput.InputController, Microsoft.Xna.Framework.Input.Keys>;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Data;
using NeoLemmixSharp.Ui.Events;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Ui.Components;

public abstract class Component : IDisposable
{
    private Component? _parent = null;
    protected List<Component>? _children = null;

    private MouseMoveEventHandler? _mouseEnter;
    private MouseMoveEventHandler? _mouseMovement;
    private MousePressEventHandler? _mousePressed;
    private MousePressEventHandler? _mouseHeld;
    private MousePressEventHandler? _mouseDoubleClick;
    private MousePressEventHandler? _mouseReleased;
    private MouseMoveEventHandler? _mouseExit;

    private KeyboardEventHandler? _keyPressed;
    private KeyboardEventHandler? _keyHeld;
    private KeyboardEventHandler? _keyReleased;

    public Point Position { get; private set; }
    public Size Dimensions { get; private set; }

    private ColorPacket _colourPacket;

    private ComponentState _state = ComponentState.Normal;

    private bool _isVisible = true;
    private bool _isDisposed;

    public MouseMoveEventHandler MouseEnter => _mouseEnter ??= new MouseMoveEventHandler();
    public MouseMoveEventHandler MouseMovement => _mouseMovement ??= new MouseMoveEventHandler();
    public MousePressEventHandler MousePressed => _mousePressed ??= new MousePressEventHandler();
    public MousePressEventHandler MouseHeld => _mouseHeld ??= new MousePressEventHandler();
    public MousePressEventHandler MouseDoubleClick => _mouseDoubleClick ??= new MousePressEventHandler();
    public MousePressEventHandler MouseReleased => _mouseReleased ??= new MousePressEventHandler();
    public MouseMoveEventHandler MouseExit => _mouseExit ??= new MouseMoveEventHandler();

    public KeyboardEventHandler KeyPressed => _keyPressed ??= new KeyboardEventHandler();
    public KeyboardEventHandler KeyHeld => _keyHeld ??= new KeyboardEventHandler();
    public KeyboardEventHandler KeyReleased => _keyReleased ??= new KeyboardEventHandler();

    protected Component()
    {
        Left = 0;
        Top = 0;

        Width = 10;
        Height = 10;
        _colourPacket = UiConstants.RectangularButtonDefaultColours;
    }

    protected Component(int x, int y)
        : this(x, y, UiConstants.StandardButtonHeight, UiConstants.StandardButtonHeight)
    {
    }

    protected Component(int x, int y, int width, int height)
    {
        Left = x;
        Top = y;

        Width = width;
        Height = height;
        _colourPacket = UiConstants.RectangularButtonDefaultColours;
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

    private void Translate(int dx, int dy)
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

    [Pure]
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

    public virtual void Tick() { }

    public virtual void Render(SpriteBatch spriteBatch)
    {
        if (_isVisible)
            RenderComponent(spriteBatch);
        RenderChildren(spriteBatch);
    }

    private void RenderChildren(SpriteBatch spriteBatch)
    {
        if (_children == null)
            return;

        for (int i = 0; i < _children.Count; i++)
        {
            _children[i].Render(spriteBatch);
        }
    }

    protected virtual void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawBeveledRectangle(spriteBatch, this);
    }

    public void AddChild(Component? c) => AddChild(c, -1);

    public void AddChild(Component? c, int index)
    {
        _children ??= new List<Component>();

        AssertValidComponentAdd(this, c);

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

    private static void AssertValidComponentAdd(Component parent, [NotNull] Component? child)
    {
        ArgumentNullException.ThrowIfNull(child);

        if (parent == child)
            throw new InvalidOperationException($"Cannot add a component to itself [{parent}]");

        if (child.IsChild())
            throw new InvalidOperationException($"Component is already a child [{child}]");
    }

    [Pure]
    internal bool IsChild() => _parent != null;

    [Pure]
    internal bool HasChildren() => _children != null;

    [Pure]
    internal Component? GetParent() => _parent;

    [Pure]
    internal Component GetTopParent()
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

    [Pure]
    internal Component? GetChildAt(Point position)
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

    internal void InvokeMouseEnter(Point mousePosition) => _mouseEnter?.Invoke(this, mousePosition);
    internal void InvokeMouseMovement(Point mousePosition) => _mouseMovement?.Invoke(this, mousePosition);
    internal void InvokeMousePressed(Point mousePosition, MouseButtonType mouseButtonType) => _mousePressed?.Invoke(this, mousePosition, mouseButtonType);
    internal void InvokeMouseHeld(Point mousePosition, MouseButtonType mouseButtonType) => _mouseHeld?.Invoke(this, mousePosition, mouseButtonType);
    internal void InvokeMouseDoubleClick(Point mousePosition, MouseButtonType mouseButtonType) => _mouseDoubleClick?.Invoke(this, mousePosition, mouseButtonType);
    internal void InvokeMouseReleased(Point mousePosition, MouseButtonType mouseButtonType) => _mouseReleased?.Invoke(this, mousePosition, mouseButtonType);
    internal void InvokeMouseExit(Point mousePosition) => _mouseExit?.Invoke(this, mousePosition);

    internal void InvokeKeyPressed(in KeysEnumerable pressedKeys) => _keyPressed?.Invoke(this, in pressedKeys);
    internal void InvokeKeyHeld(in KeysEnumerable heldKeys) => _keyHeld?.Invoke(this, in heldKeys);
    internal void InvokeKeyReleased(in KeysEnumerable releasedKeys) => _keyReleased?.Invoke(this, in releasedKeys);

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
                foreach (var child in _children)
                {
                    child.Dispose();
                }

                _children.Clear();
                _children = null;
            }

            _parent = null;

            DisposableHelperMethods.DisposeOf(ref _mouseEnter);
            DisposableHelperMethods.DisposeOf(ref _mouseMovement);
            DisposableHelperMethods.DisposeOf(ref _mousePressed);
            DisposableHelperMethods.DisposeOf(ref _mouseHeld);
            DisposableHelperMethods.DisposeOf(ref _mouseDoubleClick);
            DisposableHelperMethods.DisposeOf(ref _mouseReleased);
            DisposableHelperMethods.DisposeOf(ref _mouseExit);

            DisposableHelperMethods.DisposeOf(ref _keyPressed);
            DisposableHelperMethods.DisposeOf(ref _keyHeld);
            DisposableHelperMethods.DisposeOf(ref _keyReleased);

            UiHandler.Instance.EliminateComponentReferences(this);

            OnDispose();
        }
        GC.SuppressFinalize(this);
    }

    protected virtual void OnDispose()
    {
    }
}
