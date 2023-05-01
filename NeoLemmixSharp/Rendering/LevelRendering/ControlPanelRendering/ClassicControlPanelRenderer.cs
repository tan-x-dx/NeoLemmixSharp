using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.ControlPanel;
using NeoLemmixSharp.Rendering.Text;
using System;
using System.Linq;

namespace NeoLemmixSharp.Rendering.LevelRendering.ControlPanelRendering;

public sealed class ClassicControlPanelRenderer : IControlPanelRenderer
{
    private const int MaxNumberOfSkillButtons = 10;
    private const int ControlPanelButtonPixelWidth = 16;
    private const int ControlPanelButtonPixelHeight = 23;
    private const int ControlPanelInfoPixelHeight = 16;
    private const int ControlPanelTotalPixelHeight = ControlPanelButtonPixelHeight + ControlPanelInfoPixelHeight;

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

    private int _screenWidth;
    private int _screenHeight;

    private int _controlPanelX;
    private int _controlPanelY;
    private int _controlPanelButtonsY;

    private int _controlPanelScale = 3;
    private int _controlPanelButtonScreenWidth = ControlPanelButtonPixelWidth;
    private int _controlPanelButtonScreenHeight = ControlPanelButtonPixelHeight;
    private int _controlPanelInfoScreenHeight = ControlPanelInfoPixelHeight;
    private int _controlPanelTotalScreenHeight = ControlPanelTotalPixelHeight;

    public ClassicControlPanelRenderer(
        SpriteBank spriteBank,
        FontBank fontBank,
        LevelControlPanel levelControlPanel)
    {
        _skillAssignButtonRenderers = levelControlPanel
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

    public void SetScreenDimensions(int screenWidth, int screenHeight)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;

        // 19 = 10 skill buttons + 9 other buttons
        var horizontalButtonScreenSpace = 19 * ControlPanelButtonPixelWidth * _controlPanelScale;

        _controlPanelX = (_screenWidth - horizontalButtonScreenSpace) / 2;
        _controlPanelY = _screenHeight - (ControlPanelTotalPixelHeight * _controlPanelScale);

        _controlPanelButtonScreenWidth = ControlPanelButtonPixelWidth * _controlPanelScale;
        _controlPanelButtonScreenHeight = ControlPanelButtonPixelHeight * _controlPanelScale;
        _controlPanelInfoScreenHeight = ControlPanelInfoPixelHeight * _controlPanelScale;
        _controlPanelTotalScreenHeight = ControlPanelTotalPixelHeight * _controlPanelScale;

        _controlPanelButtonsY = _controlPanelY + _controlPanelInfoScreenHeight;
    }

    public void RenderControlPanel(SpriteBatch spriteBatch)
    {
        var destRectangle = new Rectangle(_controlPanelX, _controlPanelButtonsY, _controlPanelButtonScreenWidth, _controlPanelButtonScreenHeight);
        var panelButtonBackgroundSourceRectangle = new Rectangle(0, 0, ControlPanelButtonPixelWidth, ControlPanelButtonPixelHeight);
        spriteBatch.Draw(_skillPanels, destRectangle, panelButtonBackgroundSourceRectangle, Color.White);
        spriteBatch.Draw(_skillCountErase, destRectangle, Color.White);
        spriteBatch.Draw(_iconRrMinus, destRectangle, Color.White);

        destRectangle.X += _controlPanelButtonScreenWidth;
        panelButtonBackgroundSourceRectangle.X = ControlPanelButtonPixelWidth;

        spriteBatch.Draw(_skillPanels, destRectangle, panelButtonBackgroundSourceRectangle, Color.White);
        spriteBatch.Draw(_skillCountErase, destRectangle, Color.White);
        spriteBatch.Draw(_iconRrPlus, destRectangle, Color.White);

        destRectangle.X += _controlPanelButtonScreenWidth;
        panelButtonBackgroundSourceRectangle.X += ControlPanelButtonPixelWidth;

        var buttonLimit = Math.Min(_skillAssignButtonRenderers.Length, MaxNumberOfSkillButtons);
        var i = 3;
        var t = 0;
        for (; t < buttonLimit; t++)
        {
            _skillAssignButtonRenderers[t].RenderAtPosition(
                spriteBatch,
                destRectangle,
                panelButtonBackgroundSourceRectangle,
                _controlPanelScale);

            destRectangle.X += _controlPanelButtonScreenWidth;
            i = (i + 1) & 7;
            panelButtonBackgroundSourceRectangle.X = i * ControlPanelButtonPixelWidth;
        }

        for (; t < MaxNumberOfSkillButtons; t++)
        {
            spriteBatch.Draw(_emptySlot, destRectangle, Color.White);

            destRectangle.X += _controlPanelButtonScreenWidth;
        }

        spriteBatch.Draw(_skillPanels, destRectangle, panelButtonBackgroundSourceRectangle, Color.White);
        spriteBatch.Draw(_iconPause, destRectangle, Color.White);
        destRectangle.X += _controlPanelButtonScreenWidth;
        i = (i + 1) & 7;
        panelButtonBackgroundSourceRectangle.X = i * ControlPanelButtonPixelWidth;

        spriteBatch.Draw(_skillPanels, destRectangle, panelButtonBackgroundSourceRectangle, Color.White);
        spriteBatch.Draw(_iconNuke, destRectangle, Color.White);
        destRectangle.X += _controlPanelButtonScreenWidth;
        i = (i + 1) & 7;
        panelButtonBackgroundSourceRectangle.X = i * ControlPanelButtonPixelWidth;

        spriteBatch.Draw(_skillPanels, destRectangle, panelButtonBackgroundSourceRectangle, Color.White);
        spriteBatch.Draw(_iconFf, destRectangle, Color.White);
        destRectangle.X += _controlPanelButtonScreenWidth;
        i = (i + 1) & 7;
        panelButtonBackgroundSourceRectangle.X = i * ControlPanelButtonPixelWidth;

        spriteBatch.Draw(_skillPanels, destRectangle, panelButtonBackgroundSourceRectangle, Color.White);
        spriteBatch.Draw(_iconRestart, destRectangle, Color.White);
        destRectangle.X += _controlPanelButtonScreenWidth;
        i = (i + 1) & 7;
        panelButtonBackgroundSourceRectangle.X = i * ControlPanelButtonPixelWidth;

        spriteBatch.Draw(_skillPanels, destRectangle, panelButtonBackgroundSourceRectangle, Color.White);
        spriteBatch.Draw(_iconFrameskip, destRectangle, Color.White);
        destRectangle.X += _controlPanelButtonScreenWidth;
        i = (i + 1) & 7;
        panelButtonBackgroundSourceRectangle.X = i * ControlPanelButtonPixelWidth;

        spriteBatch.Draw(_skillPanels, destRectangle, panelButtonBackgroundSourceRectangle, Color.White);
        spriteBatch.Draw(_iconDirectional, destRectangle, Color.White);
        destRectangle.X += _controlPanelButtonScreenWidth;
        i = (i + 1) & 7;
        panelButtonBackgroundSourceRectangle.X = i * ControlPanelButtonPixelWidth;

        spriteBatch.Draw(_skillPanels, destRectangle, panelButtonBackgroundSourceRectangle, Color.White);
        spriteBatch.Draw(_iconCpmAndReplay, destRectangle, Color.White);
        destRectangle.X += _controlPanelButtonScreenWidth;
        i = (i + 1) & 7;
        panelButtonBackgroundSourceRectangle.X = i * ControlPanelButtonPixelWidth;
    }

    public void Dispose()
    {
    }
}