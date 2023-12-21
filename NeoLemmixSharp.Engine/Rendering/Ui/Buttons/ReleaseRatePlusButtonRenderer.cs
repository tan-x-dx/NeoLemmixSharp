using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

namespace NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

public sealed class ReleaseRatePlusButtonRenderer : ControlPanelButtonRenderer
{
    private readonly ReleaseRatePlusButton _releaseRatePlusButton;

    private readonly Texture2D _skillPanels;
    private readonly Texture2D _skillIcons;

    private readonly SkillCountDigitFont _skillCountDigitFont;

    public ReleaseRatePlusButtonRenderer(ControlPanelSpriteBank spriteBank, ReleaseRatePlusButton releaseRatePlusButton)
    {
        _releaseRatePlusButton = releaseRatePlusButton;

        _skillPanels = spriteBank.GetTexture(ControlPanelTexture.Panel);
        _skillIcons = spriteBank.GetTexture(ControlPanelTexture.PanelSkills);

        _skillCountDigitFont = FontBank.SkillCountDigitFont;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        var destRectangle = new Rectangle(
            _releaseRatePlusButton.ScreenX,
            _releaseRatePlusButton.ScreenY,
            _releaseRatePlusButton.ScreenWidth,
            _releaseRatePlusButton.ScreenHeight);

        spriteBatch.Draw(
            _skillPanels,
            destRectangle,
            PanelHelpers.GetRectangleForCoordinates(_releaseRatePlusButton.SkillPanelFrame, 0),
            RenderingLayers.ControlPanelButtonLayer);

        spriteBatch.Draw(
            _skillPanels,
            destRectangle,
            PanelHelpers.GetRectangleForCoordinates(1, 2),
            RenderingLayers.ControlPanelSkillCountEraseLayer);

        spriteBatch.Draw(
            _skillPanels,
            destRectangle,
            PanelHelpers.GetRectangleForCoordinates(9, 1),
            RenderingLayers.ControlPanelSkillIconLayer);

        RenderReleaseRateNumber(spriteBatch, destRectangle);
    }

    private void RenderReleaseRateNumber(SpriteBatch spriteBatch, Rectangle destRectangle)
    {

    }
}