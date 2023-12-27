using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

namespace NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

public sealed class SpawnIntervalButtonRenderer : ControlPanelButtonRenderer
{
	private readonly SpawnIntervalButton _spawnIntervalButton;

	private readonly int _iconX;
	private readonly int _iconY;

	public SpawnIntervalButtonRenderer(
		ControlPanelSpriteBank spriteBank,
		SpawnIntervalButton spawnIntervalButton)
		: base(spriteBank)
	{
		_spawnIntervalButton = spawnIntervalButton;

		var iconType = spawnIntervalButton.GetIcon();

		switch (iconType)
		{
			case SpawnIntervalButton.SpawnIntervalButtonIcon.Nothing:
				_iconX = -1;
				_iconY = -1;
				break;

			case SpawnIntervalButton.SpawnIntervalButtonIcon.MinusSign:
				_iconX = PanelHelpers.MinusButtonX;
				_iconY = PanelHelpers.ButtonIconsY;
				break;

			case SpawnIntervalButton.SpawnIntervalButtonIcon.PlusSign:
				_iconX = PanelHelpers.PlusButtonX;
				_iconY = PanelHelpers.ButtonIconsY;
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(iconType), iconType, "Unknown icon type");
		}
	}

	public override void Render(SpriteBatch spriteBatch)
	{
		var destRectangle = new Rectangle(
			_spawnIntervalButton.ScreenX,
			_spawnIntervalButton.ScreenY,
			_spawnIntervalButton.ScreenWidth,
			_spawnIntervalButton.ScreenHeight);

		spriteBatch.Draw(
			PanelTexture,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(_spawnIntervalButton.SkillPanelFrame, PanelHelpers.PanelBackgroundsY),
			RenderingLayers.ControlPanelButtonLayer);

		spriteBatch.Draw(
			PanelTexture,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(PanelHelpers.SkillIconMaskX, PanelHelpers.SkillIconMaskY),
			RenderingLayers.ControlPanelSkillCountEraseLayer);

		spriteBatch.Draw(
			PanelTexture,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(_iconX, _iconY),
			RenderingLayers.ControlPanelSkillIconLayer);

		RenderReleaseRateNumber(spriteBatch, destRectangle);
	}

	private void RenderReleaseRateNumber(SpriteBatch spriteBatch, Rectangle destRectangle)
	{
		var buttonScaleMultiplier = _spawnIntervalButton.ScaleMultiplier;
		var dx = 3 * buttonScaleMultiplier;

		FontBank.SkillCountDigitFont.RenderTextSpan(
			spriteBatch,
			_spawnIntervalButton.ReleaseRateChars,
			destRectangle.X + dx,
			destRectangle.Y + buttonScaleMultiplier,
			buttonScaleMultiplier,
			Color.White);
	}
}