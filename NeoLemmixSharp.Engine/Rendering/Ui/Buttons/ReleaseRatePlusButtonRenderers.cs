using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

namespace NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

public sealed class ReleaseRateMinusButtonRenderer : ControlPanelButtonRenderer
{
    private readonly SpawnIntervalMinusButton _spawnIntervalMinusButton;

    private readonly Texture2D _skillPanels;
    private readonly Texture2D _skillIcons;

    private readonly SkillCountDigitFont _skillCountDigitFont;

    public ReleaseRateMinusButtonRenderer(ControlPanelSpriteBank spriteBank, SpawnIntervalMinusButton spawnIntervalMinusButton)
    {
        _spawnIntervalMinusButton = spawnIntervalMinusButton;

        _skillPanels = spriteBank.GetTexture(ControlPanelTexture.Panel);
        _skillIcons = spriteBank.GetTexture(ControlPanelTexture.PanelSkills);

        _skillCountDigitFont = FontBank.SkillCountDigitFont;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        var destRectangle = new Rectangle(
            _spawnIntervalMinusButton.ScreenX,
            _spawnIntervalMinusButton.ScreenY,
            _spawnIntervalMinusButton.ScreenWidth,
            _spawnIntervalMinusButton.ScreenHeight);

        spriteBatch.Draw(
            _skillPanels,
            destRectangle,
            PanelHelpers.GetRectangleForCoordinates(_spawnIntervalMinusButton.SkillPanelFrame, 0),
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

public sealed class ReleaseRateDisplayButtonRenderer : ControlPanelButtonRenderer
{
	private readonly SpawnIntervalDisplayButton _spawnIntervalDisplayButton;

	private readonly Texture2D _skillPanels;
	private readonly Texture2D _skillIcons;

	private readonly SkillCountDigitFont _skillCountDigitFont;

	public ReleaseRateDisplayButtonRenderer(ControlPanelSpriteBank spriteBank, SpawnIntervalDisplayButton spawnIntervalDisplayButton)
	{
		_spawnIntervalDisplayButton = spawnIntervalDisplayButton;

		_skillPanels = spriteBank.GetTexture(ControlPanelTexture.Panel);
		_skillIcons = spriteBank.GetTexture(ControlPanelTexture.PanelSkills);

		_skillCountDigitFont = FontBank.SkillCountDigitFont;
	}

	public override void Render(SpriteBatch spriteBatch)
	{
		var destRectangle = new Rectangle(
			_spawnIntervalDisplayButton.ScreenX,
			_spawnIntervalDisplayButton.ScreenY,
			_spawnIntervalDisplayButton.ScreenWidth,
			_spawnIntervalDisplayButton.ScreenHeight);

		spriteBatch.Draw(
			_skillPanels,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(_spawnIntervalDisplayButton.SkillPanelFrame, 0),
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

public sealed class ReleaseRatePlusButtonRenderer : ControlPanelButtonRenderer
{
	private readonly SpawnIntervalPlusButton _spawnIntervalPlusButton;

	private readonly Texture2D _skillPanels;
	private readonly Texture2D _skillIcons;

	private readonly SkillCountDigitFont _skillCountDigitFont;

	public ReleaseRatePlusButtonRenderer(ControlPanelSpriteBank spriteBank, SpawnIntervalPlusButton spawnIntervalPlusButton)
	{
		_spawnIntervalPlusButton = spawnIntervalPlusButton;

		_skillPanels = spriteBank.GetTexture(ControlPanelTexture.Panel);
		_skillIcons = spriteBank.GetTexture(ControlPanelTexture.PanelSkills);

		_skillCountDigitFont = FontBank.SkillCountDigitFont;
	}

	public override void Render(SpriteBatch spriteBatch)
	{
		var destRectangle = new Rectangle(
			_spawnIntervalPlusButton.ScreenX,
			_spawnIntervalPlusButton.ScreenY,
			_spawnIntervalPlusButton.ScreenWidth,
			_spawnIntervalPlusButton.ScreenHeight);

		spriteBatch.Draw(
			_skillPanels,
			destRectangle,
			PanelHelpers.GetRectangleForCoordinates(_spawnIntervalPlusButton.SkillPanelFrame, 0),
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