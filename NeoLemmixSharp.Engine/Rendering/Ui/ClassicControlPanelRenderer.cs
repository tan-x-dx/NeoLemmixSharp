using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public sealed class ClassicControlPanelRenderer : IControlPanelRenderer
{
	private const int ControlPanelScaleMultiplier = 4;

	private readonly LevelControlPanel _levelControlPanel;
	private readonly ControlPanelButtonRenderer[] _controlPanelButtonRenderers;
	private readonly SkillAssignButtonRenderer[] _skillAssignButtonRenderers;

	private readonly Texture2D _panels;

	public ClassicControlPanelRenderer(
		ControlPanelSpriteBank spriteBank,
		LevelControlPanel levelControlPanel)
	{
		_levelControlPanel = levelControlPanel;

		var allButtons = _levelControlPanel.AllButtons;
		var skillAssignButtons = _levelControlPanel.SkillAssignButtons;
		_controlPanelButtonRenderers = SetUpButtonRenderers(spriteBank, allButtons);
		_skillAssignButtonRenderers = SetUpSkillButtonRenderers(skillAssignButtons);

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
			result[i++] = button.CreateButtonRenderer(spriteBank);
		}

		return result;
	}

	private SkillAssignButtonRenderer[] SetUpSkillButtonRenderers(ReadOnlySpan<SkillAssignButton> skillAssignButtons)
	{
		var result = new SkillAssignButtonRenderer[skillAssignButtons.Length];

		var i = 0;
		foreach (var controlPanelButtonRenderer in _controlPanelButtonRenderers)
		{
			if (controlPanelButtonRenderer is SkillAssignButtonRenderer skillAssignButtonRenderer)
			{
				result[i++] = skillAssignButtonRenderer;
			}
		}

		return result;
	}

	public void RenderControlPanel(SpriteBatch spriteBatch)
	{
		RenderSkillAssignButtons(spriteBatch);

		var levelTimer = _levelControlPanel.LevelTimer;
		var timerX = _levelControlPanel.ScreenWidth - PanelFont.GlyphWidth * 6 * ControlPanelScaleMultiplier;

		FontBank.PanelFont.RenderTextSpan(spriteBatch, levelTimer.AsSpan(), timerX, _levelControlPanel.ControlPanelY, ControlPanelScaleMultiplier, levelTimer.FontColor);
	}

	private void RenderSkillAssignButtons(SpriteBatch spriteBatch)
	{
		var i = _levelControlPanel.ReleaseRateButtonOffset;
		foreach (var skillAssignButtonRenderer in _skillAssignButtonRenderers)
		{
			skillAssignButtonRenderer.Render(spriteBatch);
			i++;
		}

		if (i >= LevelControlPanel.MaxNumberOfSkillButtons)
			return;

		var emptySlotSourceRectangle = PanelHelpers.GetRectangleForCoordinates(0, 2);
		var destRectangle = new Rectangle(
			_levelControlPanel.ControlPanelX + i * _levelControlPanel.ControlPanelButtonScreenWidth,
			_levelControlPanel.ControlPanelButtonY,
			_levelControlPanel.ControlPanelButtonScreenWidth,
			_levelControlPanel.ControlPanelButtonScreenHeight);
		for (; i < LevelControlPanel.MaxNumberOfSkillButtons; i++)
		{
			spriteBatch.Draw(
				_panels,
				destRectangle,
				emptySlotSourceRectangle,
				RenderingLayers.ControlPanelButtonLayer);

			destRectangle.X += _levelControlPanel.ControlPanelButtonScreenWidth;
		}
	}

	public void Dispose()
	{

	}
}