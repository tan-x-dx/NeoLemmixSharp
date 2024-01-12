using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public sealed class ControlPanelSpriteBankBuilder
{
	private readonly ContentManager _contentManager;
	private readonly GraphicsDevice _graphicsDevice;

	private readonly Texture2D[] _textureLookup;

	public ControlPanelSpriteBankBuilder(
		GraphicsDevice graphicsDevice,
		ContentManager contentManager)
	{
		_graphicsDevice = graphicsDevice;
		_contentManager = contentManager;

		var numberOfResources = Enum.GetValuesAsUnderlyingType<ControlPanelTexture>().Length;
		_textureLookup = new Texture2D[numberOfResources];
	}

	public ControlPanelSpriteBank BuildControlPanelSpriteBank()
	{
		LoadPanelTextures();

		return new ControlPanelSpriteBank(_textureLookup);
	}

	private void LoadPanelTextures()
	{
		RegisterTexture(ControlPanelTexture.Panel);
		RegisterTexture(ControlPanelTexture.PanelMinimapRegion);
		RegisterTexture(ControlPanelTexture.PanelIcons);
		RegisterTexture(ControlPanelTexture.PanelSkillSelected);
		RegisterTexture(ControlPanelTexture.PanelSkills);
	}

	private void RegisterTexture(ControlPanelTexture textureName)
	{
		var texture = _contentManager.Load<Texture2D>(textureName.GetTexturePath());
		RegisterTexture(textureName, texture);
	}

	private void RegisterTexture(ControlPanelTexture textureName, Texture2D texture)
	{
		_textureLookup[(int)textureName] = texture;
	}
}