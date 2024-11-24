using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Ui.Components.Buttons;

public class Button : Component, IModifyableState
{
    private ComponentState _state = ComponentState.Normal;

    public virtual ComponentState State
    {
        get => _state;
        set => _state = value;
    }

    public Button(int x, int y, int width, int height, string? label)
        : base(x, y, width, height, label)
    {
        MouseEnter.RegisterMouseEvent(SetMouseOver);
        MouseDown.RegisterMouseEvent(SetMousePress);
        MouseUp.RegisterMouseEvent(SetMouseOver);
        MouseExit.RegisterMouseEvent(SetMouseNormal);
    }

    public Button(int x, int y, string? label)
        : base(x, y, UiConstants.TwiceStandardInset + (int)(0.5f + (label?.Length ?? 10) * UiConstants.FontGlyphWidthMultiplier), UiConstants.StandardButtonHeight, label)
    {
        MouseEnter.RegisterMouseEvent(SetMouseOver);
        MouseDown.RegisterMouseEvent(SetMousePress);
        MouseUp.RegisterMouseEvent(SetMouseOver);
        MouseExit.RegisterMouseEvent(SetMouseNormal);
    }

    protected void SetMouseOver(Component _, LevelPosition mousePosition) => State = ComponentState.MouseOver;
    protected void SetMousePress(Component _, LevelPosition mousePosition) => State = ComponentState.MousePress;
    protected void SetMouseNormal(Component _, LevelPosition mousePosition) => State = ComponentState.Normal;

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawStateColoredBeveledRectangle(spriteBatch, this);
    }
}
