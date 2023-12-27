using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

namespace NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

public sealed class PaddingButtonRenderer : ControlPanelButtonRenderer
{
	private readonly PaddingButton _paddingButton;

	public PaddingButtonRenderer(ControlPanelSpriteBank spriteBank, PaddingButton paddingButton)
		: base(spriteBank)
	{
		_paddingButton = paddingButton;
	}

	public override void Render(SpriteBatch spriteBatch)
	{
		var destRectangle = new Rectangle(
			_paddingButton.ScreenX,
			_paddingButton.ScreenY,
			_paddingButton.ScreenWidth,
			_paddingButton.ScreenHeight);

		spriteBatch.Draw(
			PanelTexture,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(PanelHelpers.PaddingButtonX, PanelHelpers.PaddingButtonY),
			RenderingLayers.ControlPanelButtonLayer);
	}
}