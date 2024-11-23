using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Ui.Components.Buttons;

public class Button : Component, IModifyableState
{
    protected bool _isActive = false;

    public ComponentState State { get; set; } = ComponentState.Normal;

    public Button(int x, int y, int width, int height, string? label)
        : base(x, y, width, height, label)
    {
    }

    public Button(int x, int y, string? label)
        : base(x, y, UiConstants.TwiceStandardInset + (int)(0.5f + (label?.Length ?? 10) * UiConstants.FontGlyphWidthMultiplier), UiConstants.StandardButtonHeight, label)
    {
    }

    public virtual bool Active
    {
        get => _isActive;
        set
        {
            _isActive = value;
            State = _isActive ? ComponentState.Active : ComponentState.Normal;
        }
    }

    public override void InvokeMouseEnter(LevelPosition mousePosition) => State = ComponentState.MouseOver;

    public override void InvokeMouseDown(LevelPosition mousePosition)
    {
        State = ComponentState.MousePress;
        Click();
    }

    public override void InvokeMouseUp(LevelPosition mousePosition) => State = ComponentState.MouseOver;

    public override void InvokeMouseExit(LevelPosition mousePosition) => State = ComponentState.Normal;

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawBeveledRectangle(spriteBatch, this);
    }
}
