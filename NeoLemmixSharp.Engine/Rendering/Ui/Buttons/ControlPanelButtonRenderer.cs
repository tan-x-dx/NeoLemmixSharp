using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

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

		PanelTexture = spriteBank.GetTexture(ControlPanelTexture.Panel);
		SelectedTexture = spriteBank.GetTexture(ControlPanelTexture.PanelSkillSelected);

		_iconX = iconX;
		_iconY = iconY;
	}

	protected Rectangle GetDestinationRectangle()
	{
		return new Rectangle(
			ControlPanelButton.ScreenX,
			ControlPanelButton.ScreenY,
			ControlPanelButton.ScreenWidth,
			ControlPanelButton.ScreenHeight);
	}

	public virtual void Render(SpriteBatch spriteBatch)
	{
		var destRectangle = GetDestinationRectangle();

		spriteBatch.Draw(
			PanelTexture,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(ControlPanelButton.SkillPanelFrame, PanelHelpers.PanelBackgroundY),
			RenderingLayers.ControlPanelButtonLayer);

		RenderDigits(spriteBatch, destRectangle);

		spriteBatch.Draw(
			PanelTexture,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(_iconX, _iconY),
			RenderingLayers.ControlPanelSkillCountEraseLayer);

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
			PanelHelpers.GetRectangleForCoordinates(iconX, PanelHelpers.SkillIconMaskY),
			RenderingLayers.ControlPanelSkillCountEraseLayer);

		var buttonScaleMultiplier = LevelScreen.LevelControlPanel.ControlPanelScale;
		var dx = 3 * buttonScaleMultiplier;

		FontBank.SkillCountDigitFont.RenderTextSpan(
			spriteBatch,
			ControlPanelButton.GetDigitsToRender(),
			destRectangle.X + dx,
			destRectangle.Y + buttonScaleMultiplier,
			buttonScaleMultiplier,
			Color.White);
	}

	protected void RenderSelected(SpriteBatch spriteBatch, Rectangle destRectangle)
	{
		if (!ControlPanelButton.IsSelected)
			return;

		spriteBatch.Draw(
			SelectedTexture,
			destRectangle,
			new Rectangle(0, 0, SelectedTexture.Width, SelectedTexture.Height),
			RenderingLayers.ControlPanelSkillCountEraseLayer); // Can reuse this layer since the sprites shouldn't overlap anyway
	}
}