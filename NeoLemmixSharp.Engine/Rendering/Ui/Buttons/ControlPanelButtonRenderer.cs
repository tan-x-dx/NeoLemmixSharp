using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

public abstract class ControlPanelButtonRenderer
{
	protected Texture2D PanelTexture;

	protected ControlPanelButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		PanelTexture = spriteBank.GetTexture(ControlPanelTexture.Panel);
	}

    public abstract void Render(SpriteBatch spriteBatch);
}