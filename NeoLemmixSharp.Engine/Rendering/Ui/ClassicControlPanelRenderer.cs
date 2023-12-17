using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.Level.ControlPanel;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public sealed class ClassicControlPanelRenderer : IControlPanelRenderer
{
    private const int ControlPanelScaleMultiplier = 4;

    private readonly LevelControlPanel _levelControlPanel;
    private readonly SkillAssignButtonRenderer[] _skillAssignButtonRenderers;

    private readonly Texture2D _whitePixelTexture;
    private readonly Texture2D _panels;

    public ClassicControlPanelRenderer(
        ControlPanelSpriteBank spriteBank,
        LevelControlPanel levelControlPanel)
    {
        _levelControlPanel = levelControlPanel;
        _skillAssignButtonRenderers = _levelControlPanel
            .SkillAssignButtons
            .Select(b => new SkillAssignButtonRenderer(spriteBank, b))
            .ToArray();

        _whitePixelTexture = spriteBank.GetTexture(ControlPanelTexture.WhitePixel);
        _panels = spriteBank.GetTexture(ControlPanelTexture.Panel);
    }

    public void RenderControlPanel(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _whitePixelTexture,
            new Rectangle(
                0,
                _levelControlPanel.ControlPanelY,
                _levelControlPanel.ScreenWidth,
                _levelControlPanel.ControlPanelScreenHeight),
            new Rectangle(0, 0, 1, 1),
            Color.Black,
            RenderingLayers.ControlPanelBackgroundLayer);

        var i = 0;
        for (; i < _skillAssignButtonRenderers.Length; i++)
        {
            _skillAssignButtonRenderers[i].Render(spriteBatch);
        }

        if (i < LevelControlPanel.MaxNumberOfSkillButtons)
        {
            var emptySlotSourceRectangle = PanelHelpers.GetRectangleForCoordinates(0, 2);
            var destRectangle = new Rectangle(
                _levelControlPanel.ControlPanelX + (i + 4) * _levelControlPanel.ControlPanelButtonScreenWidth,
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

        var levelTimer = _levelControlPanel.LevelTimer;
        var timerX = _levelControlPanel.ScreenWidth - PanelFont.GlyphWidth * 6 * ControlPanelScaleMultiplier;

        FontBank.PanelFont.RenderTextSpan(spriteBatch, levelTimer.AsSpan(), timerX, _levelControlPanel.ControlPanelY, ControlPanelScaleMultiplier, levelTimer.FontColor);
    }

    public void Dispose()
    {

    }
}