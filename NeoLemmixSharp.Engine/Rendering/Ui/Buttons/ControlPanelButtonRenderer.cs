using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

public class ControlPanelButtonRenderer
{
    protected readonly ControlPanelButton ControlPanelButton;

    protected readonly Texture2D PanelTexture;
    protected readonly Texture2D SelectedTexture;

    private readonly int _iconX;
    private readonly int _iconY;

    public ControlPanelButtonRenderer(
        ControlPanelSpriteBank spriteBank,
        ControlPanelButton controlPanelButton,
        int iconX,
        int iconY)
    {
        ControlPanelButton = controlPanelButton;

        PanelTexture = spriteBank.Panel;
        SelectedTexture = spriteBank.PanelSkillSelected;

        _iconX = iconX;
        _iconY = iconY;
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected Rectangle GetDestinationRectangle()
    {
        return new Rectangle(
            ControlPanelButton.X,
            ControlPanelButton.Y,
            ControlPanelButton.Width,
            ControlPanelButton.Height);
    }

    public virtual void Render(SpriteBatch spriteBatch)
    {
        var destRectangle = GetDestinationRectangle();

        spriteBatch.Draw(
            PanelTexture,
            destRectangle,
            PanelHelpers.GetRectangleForCoordinates(ControlPanelButton.SkillPanelFrame, PanelHelpers.PanelBackgroundY));

        RenderDigits(spriteBatch, destRectangle);

        spriteBatch.Draw(
            PanelTexture,
            destRectangle,
            PanelHelpers.GetRectangleForCoordinates(_iconX, _iconY));

        RenderSelected(spriteBatch, destRectangle);
    }

    protected void RenderDigits(SpriteBatch spriteBatch, Rectangle destRectangle)
    {
        var numberOfDigitsToRender = ControlPanelButton.GetNumberOfDigitsToRender();
        if (numberOfDigitsToRender == 0)
            return;

        var iconX = numberOfDigitsToRender == 3
            ? PanelHelpers.SkillIconTripleMaskX
            : PanelHelpers.SkillIconDoubleMaskX;

        spriteBatch.Draw(
            PanelTexture,
            destRectangle,
            PanelHelpers.GetRectangleForCoordinates(iconX, PanelHelpers.SkillIconMaskY));

        FontBank.SkillCountDigitFont.RenderTextSpan(
            spriteBatch,
            ControlPanelButton.GetDigitsToRender(),
            destRectangle.X + 3,
            destRectangle.Y + 1,
            1,
            Color.White);
    }

    protected void RenderSelected(SpriteBatch spriteBatch, Rectangle destRectangle)
    {
        if (!ControlPanelButton.IsSelected)
            return;

        spriteBatch.Draw(
            SelectedTexture,
            destRectangle,
            SelectedTexture.Bounds);
    }
}