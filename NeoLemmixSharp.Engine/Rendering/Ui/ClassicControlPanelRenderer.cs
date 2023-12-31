using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public sealed class ClassicControlPanelRenderer : IControlPanelRenderer
{
	private const int ControlPanelScaleMultiplier = 4;

	private readonly LevelControlPanel _levelControlPanel;
	private readonly ControlPanelButtonRenderer[] _controlPanelButtonRenderers;

	private readonly Texture2D _panels;

	public ClassicControlPanelRenderer(
		ControlPanelSpriteBank spriteBank,
		LevelControlPanel levelControlPanel)
	{
		_levelControlPanel = levelControlPanel;

		var allButtons = _levelControlPanel.AllButtons;
		_controlPanelButtonRenderers = SetUpButtonRenderers(spriteBank, allButtons);

		_panels = spriteBank.GetTexture(ControlPanelTexture.Panel);
	}

	private static ControlPanelButtonRenderer[] SetUpButtonRenderers(
		ControlPanelSpriteBank spriteBank,
		ReadOnlySpan<ControlPanelButton> allButtons)
	{
		var result = new ControlPanelButtonRenderer[allButtons.Length];

		var i = 0;
		foreach (var button in allButtons)
		{
			if (button is null)
				continue;
			try
			{
				result[i++] = button.CreateButtonRenderer(spriteBank);
			}
			catch { }
		}

		return result;
	}

	public void RenderControlPanel(SpriteBatch spriteBatch)
	{
		RenderSkillAssignButtons(spriteBatch);

		var levelTimer = _levelControlPanel.LevelTimer;
		var timerX = _levelControlPanel.ScreenWidth - PanelFont.GlyphWidth * LevelTimer.NumberOfChars * ControlPanelScaleMultiplier;

		FontBank.PanelFont.RenderTextSpan(
			spriteBatch,
			levelTimer.AsSpan(),
			timerX,
			_levelControlPanel.ControlPanelY,
			ControlPanelScaleMultiplier,
			levelTimer.FontColor);
	}

	private void RenderSkillAssignButtons(SpriteBatch spriteBatch)
	{
		foreach (var controlPanelButtonRenderer in _controlPanelButtonRenderers)
		{
			if(controlPanelButtonRenderer is null) continue;
			controlPanelButtonRenderer.Render(spriteBatch);
		}
	}

	public void Dispose()
	{

	}
}