using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Ui.Data;
using NeoLemmixSharp.Ui.Events;
using Point = NeoLemmixSharp.Common.Point;

namespace NeoLemmixSharp.Ui.Components;

public sealed class CheckBox : Component
{
    private GenericEventHandler? _onUnchecked;
    private GenericEventHandler? _onChecked;

    private ColorPacket _uncheckedColors;
    private ColorPacket _checkedColors;

    public bool IsChecked { get; private set; }

    public CheckBox()
    {
        Width = 32;
        Height = 32;

        _uncheckedColors = UiConstants.RectangularButtonDefaultColours;
        _checkedColors = new ColorPacket(
            0xff114411.AsAbgrColor(),
            0xff226622.AsAbgrColor(),
            0xff118811.AsAbgrColor(),
            0xff006600.AsAbgrColor());

        MouseEnter.RegisterMouseMoveEvent(SetMouseOver);
        MousePressed.RegisterMousePressEvent(SetMousePress, MouseButtonType.Left);
        MouseReleased.RegisterMousePressEvent(SetMouseOver, MouseButtonType.Left);
        MouseExit.RegisterMouseMoveEvent(SetMouseNormal);

        MouseReleased.RegisterMousePressEvent(OnMouseReleased, MouseButtonType.Left);
    }

    public GenericEventHandler OnUnchecked => _onUnchecked ??= new GenericEventHandler();
    public GenericEventHandler OnChecked => _onChecked ??= new GenericEventHandler();

    private void OnMouseReleased(Component c, Point position)
    {
        ToggleCheckedStatus();
    }

    public void ToggleCheckedStatus()
    {
        SetCheckedValue(!IsChecked);
    }

    public void SetCheckedValue(bool isChecked)
    {
        if (isChecked)
        {
            SetChecked();
        }
        else
        {
            SetUnchecked();
        }
    }

    public void SetChecked()
    {
        if (IsChecked)
            return;

        IsChecked = true;
        Colors = _checkedColors;
        _onChecked?.Invoke(this);
    }

    public void SetUnchecked()
    {
        if (!IsChecked)
            return;

        IsChecked = false;
        Colors = _uncheckedColors;
        _onUnchecked?.Invoke(this);
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        UiSprites.DrawBeveledRectangle(spriteBatch, this);

        if (!IsChecked)
            return;

        var checkMarkTexture = UiSprites.CheckMarkTexture;
        var destination = new Rectangle(Left - 4, Top, 32, 32);
        spriteBatch.Draw(checkMarkTexture, destination, Color.White);
    }

    protected override void OnDispose()
    {
        DisposableHelperMethods.DisposeOf(ref _onUnchecked);
        DisposableHelperMethods.DisposeOf(ref _onChecked);
    }
}
