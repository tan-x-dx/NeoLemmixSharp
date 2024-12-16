﻿global using KeysEnumerable = NeoLemmixSharp.Common.Util.Collections.SimpleSetEnumerable<NeoLemmixSharp.Common.Util.GameInput.InputController, Microsoft.Xna.Framework.Input.Keys>;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Data;
using NeoLemmixSharp.Ui.Events;

namespace NeoLemmixSharp.Ui.Components;

public abstract class Component : IDisposable
{
    public delegate void ComponentKeyboardAction(Component c, in KeysEnumerable keys);

    private int _x, _y;

    private int _width, _height;

    private ComponentState _state = ComponentState.Normal;

    private ColorPacket _colourPacket;

    private string? _textLabel = null;
    private int _labelOffsetX, _labelOffsetY;

    private bool _isVisible = true;
    private bool _isDisposed;

    private Component? _parent = null;
    protected List<Component>? _children = null;

    public MouseEventHandler MouseEnter { get; } = new();
    public MouseEventHandler MouseMovement { get; } = new();
    public MouseEventHandler MouseDown { get; } = new();
    public MouseEventHandler MouseDoubleClick { get; } = new();
    public MouseEventHandler MouseUp { get; } = new();
    public MouseEventHandler MouseExit { get; } = new();

    public KeyboardEventHandler KeyDown { get; } = new();
    public KeyboardEventHandler KeyUp { get; } = new();

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
    }

    public virtual int Width
    {
        get => _width;
        set => _width = value;
    }

    public virtual int Height
    {
        get => _height;
        set => _height = value;
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
        _width = w;
        _height = h;
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

    protected virtual void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawBeveledRectangle(spriteBatch, this);
    }

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

    public void InvokeMouseEnter(LevelPosition mousePosition) => MouseEnter?.Invoke(this, mousePosition);
    public void InvokeMouseMovement(LevelPosition mousePosition) => MouseMovement?.Invoke(this, mousePosition);
    public void InvokeMouseDown(LevelPosition mousePosition) => MouseDown?.Invoke(this, mousePosition);
    public void InvokeMouseDoubleClick(LevelPosition mousePosition) => MouseDoubleClick?.Invoke(this, mousePosition);
    public void InvokeMouseUp(LevelPosition mousePosition) => MouseUp?.Invoke(this, mousePosition);
    public void InvokeMouseExit(LevelPosition mousePosition) => MouseExit?.Invoke(this, mousePosition);

    public void InvokeKeyDown(in KeysEnumerable pressedKeys) => KeyDown?.Invoke(this, in pressedKeys);
    public void InvokeKeyUp(in KeysEnumerable pressedKeys) => KeyUp?.Invoke(this, in pressedKeys);

    protected void SetMouseOver(Component _, LevelPosition mousePosition) => State = ComponentState.MouseOver;
    protected void SetMousePress(Component _, LevelPosition mousePosition) => State = ComponentState.MousePress;
    protected void SetMouseNormal(Component _, LevelPosition mousePosition) => State = ComponentState.Normal;

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

            MouseEnter.Clear();
            MouseMovement.Clear();
            MouseDown.Clear();
            MouseDoubleClick.Clear();
            MouseUp.Clear();
            MouseExit.Clear();

            KeyDown.Clear();
            KeyUp.Clear();

            OnDispose();
            _isDisposed = true;
        }
        GC.SuppressFinalize(this);
    }

    protected virtual void OnDispose()
    {
    }
}