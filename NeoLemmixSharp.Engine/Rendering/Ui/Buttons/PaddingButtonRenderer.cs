using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

namespace NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

public sealed class PaddingButtonRenderer : ControlPanelButtonRenderer
{
	public PaddingButtonRenderer(ControlPanelSpriteBank spriteBank, PaddingButton paddingButton)
		: base(spriteBank, paddingButton, 8, 0)
	{
	}

	public override void Render(SpriteBatch spriteBatch)
	{
		var destRectangle = GetDestinationRectangle();

		spriteBatch.Draw(
			PanelTexture,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(PanelHelpers.PaddingButtonX, PanelHelpers.PaddingButtonY),
			RenderingLayers.ControlPanelButtonLayer);
	}
}