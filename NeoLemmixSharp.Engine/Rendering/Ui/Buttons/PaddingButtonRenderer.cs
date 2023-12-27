using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

namespace NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

public sealed class PaddingButtonRenderer : ControlPanelButtonRenderer
{
	public PaddingButtonRenderer(ControlPanelSpriteBank spriteBank, PaddingButton paddingButton)
		: base(spriteBank, paddingButton, 0, 0)
	{
	}

	public override void Render(SpriteBatch spriteBatch)
	{
		var destRectangle = new Rectangle(
			ControlPanelButton.ScreenX,
			ControlPanelButton.ScreenY,
			ControlPanelButton.ScreenWidth,
			ControlPanelButton.ScreenHeight);

		spriteBatch.Draw(
			PanelTexture,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(PanelHelpers.PaddingButtonX, PanelHelpers.PaddingButtonY),
			RenderingLayers.ControlPanelButtonLayer);
	}
}