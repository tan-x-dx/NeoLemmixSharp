using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class ExceptionViewer : PopupMenu
{
    private readonly Exception _exception;

    public ExceptionViewer(Exception exception)
    {
        _exception = exception;

        Width = 800;
        Height = 600;
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawBeveledRectangle(spriteBatch, this);

        var position = new Vector2(Left + UiConstants.DefaultTextXOffset, Top + UiConstants.DefaultTextYOffset);

        spriteBatch.DrawString(
            UiSprites.UiFont,
            _exception.Message,
            position,
            Color.White);

        position.Y = Top + 32 + UiConstants.DefaultTextYOffset;

        spriteBatch.DrawString(
            UiSprites.UiFont,
            _exception.StackTrace,
            position,
            Color.White);
    }
}