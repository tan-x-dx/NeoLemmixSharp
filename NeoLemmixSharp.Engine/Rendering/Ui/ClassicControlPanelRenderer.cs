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
    private readonly Texture2D _emptySlot;
    private readonly Texture2D _iconCpmAndReplay;
    private readonly Texture2D _iconDirectional;
    private readonly Texture2D _iconFf;
    private readonly Texture2D _iconFrameskip;
    private readonly Texture2D _iconNuke;
    private readonly Texture2D _iconPause;
    private readonly Texture2D _iconRestart;
    private readonly Texture2D _iconRrMinus;
    private readonly Texture2D _iconRrPlus;
    private readonly Texture2D _minimapRegion;
    private readonly Texture2D _panelIcons;
    private readonly Texture2D _skillCountErase;
    private readonly Texture2D _skillPanels;
    private readonly Texture2D _skillSelected;

    public ClassicControlPanelRenderer(
        ControlPanelSpriteBank spriteBank,
        LevelControlPanel levelControlPanel)
    {
        _levelControlPanel = levelControlPanel;
        _skillAssignButtonRenderers = _levelControlPanel
            .SkillAssignButtons
            .Select(b => new SkillAssignButtonRenderer(spriteBank, b))
            .ToArray();

        _whitePixelTexture = spriteBank.GetTexture("WhitePixel");
        _emptySlot = spriteBank.GetTexture("panel/empty_slot");
        _iconCpmAndReplay = spriteBank.GetTexture("panel/icon_cpm_and_replay");
        _iconDirectional = spriteBank.GetTexture("panel/icon_directional");
        _iconFf = spriteBank.GetTexture("panel/icon_ff");
        _iconFrameskip = spriteBank.GetTexture("panel/icon_frameskip");
        _iconNuke = spriteBank.GetTexture("panel/icon_nuke");
        _iconPause = spriteBank.GetTexture("panel/icon_pause");
        _iconRestart = spriteBank.GetTexture("panel/icon_restart");
        _iconRrMinus = spriteBank.GetTexture("panel/icon_rr_minus");
        _iconRrPlus = spriteBank.GetTexture("panel/icon_rr_plus");
        _minimapRegion = spriteBank.GetTexture("panel/minimap_region");
        _panelIcons = spriteBank.GetTexture("panel/panel_icons");
        _skillCountErase = spriteBank.GetTexture("panel/skill_count_erase");
        _skillPanels = spriteBank.GetTexture("panel/skill_panels");
        _skillSelected = spriteBank.GetTexture("panel/skill_selected");
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
            var sourceRectangle = new Rectangle(0, 0, _emptySlot.Width, _emptySlot.Height);
            var destRectangle = new Rectangle(
                _levelControlPanel.ControlPanelX + (i + 4) * _levelControlPanel.ControlPanelButtonScreenWidth,
                _levelControlPanel.ControlPanelButtonY,
                _levelControlPanel.ControlPanelButtonScreenWidth,
                _levelControlPanel.ControlPanelButtonScreenHeight);
            for (; i < LevelControlPanel.MaxNumberOfSkillButtons; i++)
            {
                spriteBatch.Draw(
                    _emptySlot,
                    destRectangle,
                    sourceRectangle,
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
        _whitePixelTexture.Dispose();
        _emptySlot.Dispose();
        _iconCpmAndReplay.Dispose();
        _iconDirectional.Dispose();
        _iconFf.Dispose();
        _iconFrameskip.Dispose();
        _iconNuke.Dispose();
        _iconPause.Dispose();
        _iconRestart.Dispose();
        _iconRrMinus.Dispose();
        _iconRrPlus.Dispose();
        _minimapRegion.Dispose();
        _panelIcons.Dispose();
        _skillCountErase.Dispose();
        _skillPanels.Dispose();
        _skillSelected.Dispose();
    }
}