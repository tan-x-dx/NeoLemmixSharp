using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.ControlPanel;
using NeoLemmixSharp.Rendering.Text;
using System.Linq;

namespace NeoLemmixSharp.Rendering.LevelRendering.ControlPanelRendering;

public sealed class ClassicControlPanelRenderer : IControlPanelRenderer
{
    private readonly LevelControlPanel _levelControlPanel;

    private readonly SkillAssignButtonRenderer[] _skillAssignButtonRenderers;

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
        SpriteBank spriteBank,
        FontBank fontBank,
        LevelControlPanel levelControlPanel)
    {
        _levelControlPanel = levelControlPanel;
        _skillAssignButtonRenderers = _levelControlPanel
            .SkillAssignButtons
            .Select(b => new SkillAssignButtonRenderer(spriteBank, fontBank, b))
            .ToArray();

        _emptySlot = spriteBank.TextureLookup["panel/empty_slot"];
        _iconCpmAndReplay = spriteBank.TextureLookup["panel/icon_cpm_and_replay"];
        _iconDirectional = spriteBank.TextureLookup["panel/icon_directional"];
        _iconFf = spriteBank.TextureLookup["panel/icon_ff"];
        _iconFrameskip = spriteBank.TextureLookup["panel/icon_frameskip"];
        _iconNuke = spriteBank.TextureLookup["panel/icon_nuke"];
        _iconPause = spriteBank.TextureLookup["panel/icon_pause"];
        _iconRestart = spriteBank.TextureLookup["panel/icon_restart"];
        _iconRrMinus = spriteBank.TextureLookup["panel/icon_rr_minus"];
        _iconRrPlus = spriteBank.TextureLookup["panel/icon_rr_plus"];
        _minimapRegion = spriteBank.TextureLookup["panel/minimap_region"];
        _panelIcons = spriteBank.TextureLookup["panel/panel_icons"];
        _skillCountErase = spriteBank.TextureLookup["panel/skill_count_erase"];
        _skillPanels = spriteBank.TextureLookup["panel/skill_panels"];
        _skillSelected = spriteBank.TextureLookup["panel/skill_selected"];
    }

    public void RenderControlPanel(SpriteBatch spriteBatch)
    {
        var i = 0;
        for (; i < _skillAssignButtonRenderers.Length; i++)
        {
            _skillAssignButtonRenderers[i].Render(spriteBatch);
        }

        if (i < LevelControlPanel.MaxNumberOfSkillButtons)
        {
            var destRectangle = new Rectangle(
                _levelControlPanel.ControlPanelX + ((i + 4) * _levelControlPanel.ControlPanelButtonScreenWidth),
                _levelControlPanel.ControlPanelButtonY,
                _levelControlPanel.ControlPanelButtonScreenWidth,
                _levelControlPanel.ControlPanelButtonScreenHeight);
            for (; i < LevelControlPanel.MaxNumberOfSkillButtons; i++)
            {
                spriteBatch.Draw(_emptySlot, destRectangle, Color.White);

                destRectangle.X += _levelControlPanel.ControlPanelButtonScreenWidth;
            }
        }
    }

    public void Dispose()
    {
    }
}